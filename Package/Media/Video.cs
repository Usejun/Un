using SkiaSharp;
using FFMpegCore;

namespace Un.Package;

public class Video : Ref<IMediaAnalysis>, IStatic
{
    public Str Path { get; private set; }

    public Video() : this(null!) { }

    public Video(IMediaAnalysis value) : base("video", value) {} 

    public override void Init()
    {        
        field.Set("duration", new NativeFun("duration", 0, field => 
        {
            if (!field[Literals.Self].As<Video>(out var self))
                throw new ArgumentError();
            return new Float(self.Value.Duration.TotalSeconds);
        }, []));

        field.Set("width", new NativeFun("width", 0, field => 
        {
            if (!field[Literals.Self].As<Video>(out var self))
                throw new ArgumentError();

            return new Int(self.Value.PrimaryVideoStream.Width);
        }, []));

        field.Set("height", new NativeFun("height", 0, field => 
        {
            if (!field[Literals.Self].As<Video>(out var self))
                throw new ArgumentError();

            return new Int(self.Value.PrimaryVideoStream.Height);
        }, []));

        field.Set("fps", new NativeFun("fps", 0, field => 
        {
            if (!field[Literals.Self].As<Video>(out var self))
                throw new ArgumentError();

            return new Float(self.Value.PrimaryVideoStream.FrameRate);
        }, []));

        field.Set("codec", new NativeFun("codec", 0, field => 
        {
            if (!field[Literals.Self].As<Video>(out var self))
                throw new ArgumentError();

            return new Str(self.Value.PrimaryVideoStream?.CodecName);
        }, []));

        field.Set("convert", new NativeFun("convert", 2, field => 
        {
            if (!field[Literals.Self].As<Video>(out var self))
                throw new ArgumentError();
            if (!field["format"].As<Str>(out var format))
                throw new ArgumentError();
            if (!field["output"].As<Str>(out var output))
                throw new ArgumentError();

            var inputPath = self.Path.Value;
            FFMpegArguments
                .FromFileInput(inputPath)
                .OutputToFile(output.Value, overwrite: true, options => options
                    .WithCustomArgument($"-c:v libx264 -preset slow -crf 23 -c:a aac -b:a 128k"))
                .ProcessSynchronously();

            return new Video(FFProbe.Analyse(output.Value))
            {
                Path = output
            };
        }, [("format", null!), ("output", null!)]));

        field.Set("resize", new NativeFun("resize", 3, field => 
        {
            if (!field[Literals.Self].As<Video>(out var self))
                throw new ArgumentError();
            if (!field["width"].As<Int>(out var width))
                throw new ArgumentError();
            if (!field["height"].As<Int>(out var height))
                throw new ArgumentError();
            if (!field["output"].As<Str>(out var output))
                throw new ArgumentError();

            var inputPath = self.Path.Value;
            FFMpegArguments
                .FromFileInput(inputPath)
                .OutputToFile(output.Value, overwrite: true, options => options
                    .Resize((int)width.Value, (int)height.Value)
                    .WithVideoCodec("libx264")
                    .WithAudioCodec("aac")
                    .WithFramerate(self.Value.PrimaryVideoStream.FrameRate))                    
                .ProcessSynchronously();

            return new Video(FFProbe.Analyse(output.Value))
            {
                Path = output
            };
        }, [("width", null!), ("height", null!), ("output", null!)]));

        field.Set("thumbnail", new NativeFun("thumbnail", 1, field =>
        {
            if (!field[Literals.Self].As<Video>(out var self))
                throw new ArgumentError();
            if (!field["time"].As<Float>(out _) &&
                !field["time"].As<Int>(out _))
                throw new ArgumentError();

            var time = field["time"].CFloat();
            var output = System.IO.Path.GetTempFileName() + ".jpg";
            var input = self.Path;

            FFMpegArguments
                .FromFileInput(input.Value)
                .OutputToFile(output, overwrite: true, options => options
                    .WithCustomArgument($"-vf \"select='gte(n\\,{time.Value * self.Value.PrimaryVideoStream.FrameRate})'\" -frames:v 1"))
                .ProcessSynchronously();

            using var stream = new SKFileStream(output);
            var bitmap = SKBitmap.Decode(stream);

            return new Image(bitmap ?? throw new ValueError("failed to load image"));
        }, [("time", null!)]));

        field.Set("concat", new NativeFun("concat", 2, field =>
        {
            if (!field["videos"].As<List>(out var videos))
                throw new ArgumentError();
            if (!field["output"].As<Str>(out var output))
                throw new ArgumentError();

            var tempFile = System.IO.Path.GetTempFileName();
            using (var writer = new StreamWriter(tempFile))
            {
                foreach (var videoObj in videos)
                {
                    if (!videoObj.As<Video>(out var video))
                        throw new ArgumentError("list element is only video");
                    writer.WriteLine($"file '{video.Path}'");
                }
            }

            FFMpegArguments
                .FromFileInput(tempFile, verifyExists: false)
                .OutputToFile(output.Value, overwrite: true, options => options
                    .WithCustomArgument("-f concat -safe 0 -c copy"))
                .ProcessSynchronously();

            return new Video(FFProbe.Analyse(output.Value))
            {
                Path = output
            };
        }, [("videos", null!), ("output", null!)]));

        field.Set("trim", new NativeFun("trim", 3, field =>
        {
            if (!field[Literals.Self].As<Video>(out var self))
                throw new ArgumentError();
            if (!field["start"].As<Float>(out var start))
                throw new ArgumentError();
            if (!field["duration"].As<Float>(out var duration))
                throw new ArgumentError();
            if (!field["output"].As<Str>(out var output))
                throw new ArgumentError();

            var inputPath = self.Path;

            FFMpegArguments
                .FromFileInput(inputPath.Value)
                .OutputToFile(output.Value, overwrite: true, options => options
                    .WithCustomArgument($"-ss {start.Value} -t {duration.Value} -c copy"))
                .ProcessSynchronously();

            return new Video(FFProbe.Analyse(output.Value))
            {
                Path = output
            };;
        }, [("start", null!), ("duration", null!), ("output", null!)]));

        field.Set("extract_audio", new NativeFun("extract_audio", 1, field =>
        {
            if (!field[Literals.Self].As<Video>(out var self))
                throw new ArgumentError();
            if (!field["output"].As<Str>(out var output))
                throw new ArgumentError();
        
            var inputPath = self.Path;

            FFMpegArguments
                .FromFileInput(inputPath.Value)
                .OutputToFile(output.Value, overwrite: true, options => options
                    .WithCustomArgument("-vn -acodec copy"))
                .ProcessSynchronously();

            return new Str(output.Value);
        }, [("output", null!)]));

        field.Set("replace_audio", new NativeFun("replace_audio", 2, field =>
        {
            if (!field[Literals.Self].As<Video>(out var self))
                throw new ArgumentError();
            if (!field["audio"].As<Str>(out var audio))
                throw new ArgumentError();
            if (!field["output"].As<Str>(out var output))
                throw new ArgumentError();

            var inputPath = self.Path;

            var ffmpeg = FFMpegArguments.FromFileInput(inputPath.Value);
            ffmpeg.AddFileInput(audio.Value);

            ffmpeg.OutputToFile(output.Value, overwrite: true, options => options
                .WithCustomArgument("-map 0:v:0 -map 1:a:0 -c:v copy -c:a aac -strict experimental"))
            .ProcessSynchronously();

            return new Video(FFProbe.Analyse(output.Value));
        }, [("audio", null!), ("output", null!)]));

        field.Set("key_frames", new NativeFun("key_frames", 0, field =>
        {
            if (!field[Literals.Self].As<Video>(out var self))
                throw new ArgumentError();

            var tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var outputPattern = System.IO.Path.Combine(tempDir, "keyframe_%03d.jpg");

            FFMpegArguments
                .FromFileInput(self.Path.Value)
                .OutputToFile(outputPattern, overwrite: true, options => options
                    .WithCustomArgument("-vf \"select=eq(pict_type\\,I)\" -vsync vfr"))
                .ProcessSynchronously();

            var images = new List();
            foreach (var file in Directory.GetFiles(tempDir, "keyframe_*.jpg"))
            {
                using var stream = new SKFileStream(file);
                var bitmap = SKBitmap.Decode(stream);
                if (bitmap != null)
                    images.Append(new Image(bitmap));
            }

            return images;
        }, []));
    }

    public override Obj Init(Collections.Tuple args, Field field) => throw new ClassError("cannot be created. use video.new or video.load");

    public override Str CStr() => new();

    public override int GetHashCode() => -1;

    public override Obj Copy() => this;

    public Obj Static()
    {
        Obj video = new(ClassName);
        video.field.Set("load", new NativeFun("load", 1, field => 
        {
            if (!field["path"].As<Str>(out var path))
                throw new ArgumentError();

            var fullPath = System.IO.Path.GetFullPath(Un.Process.Path + path.Value);

            return new Video(FFProbe.Analyse(fullPath)) 
            { 
                Path = new(fullPath)
            };
       
        }, [("path", null!)]));

        return video;
    }
}

