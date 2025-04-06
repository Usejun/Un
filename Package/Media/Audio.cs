using FFMpegCore;
using Un;

namespace Un.Package;

public class Audio : Ref<IMediaAnalysis>, IStatic
{
    public Str Path { get; private set; }

    public Audio() : this(null!) { }

    public Audio(IMediaAnalysis value) : base("audio", value) {}

    public override void Init()
    {        
        field.Set("duration", new NativeFun("duration", 0, field => 
        {
            if (!field[Literals.Self].As<Audio>(out var self))
                throw new ArgumentError();

            return new Float(self.Value.Duration.TotalSeconds);
        }, []));

        field.Set("bitrate", new NativeFun("bitrate", 0, field => 
        {
            if (!field[Literals.Self].As<Audio>(out var self))
                throw new ArgumentError();

            return new Int(self.Value.PrimaryAudioStream?.BitRate ?? 0);
        }, []));

        field.Set("sample_rate", new NativeFun("sample_rate", 0, field => 
        {
            if (!field[Literals.Self].As<Audio>(out var self))
                throw new ArgumentError();

            return new Int(self.Value.PrimaryAudioStream.SampleRateHz);
        }, []));

        field.Set("channels", new NativeFun("channels", 0, field => 
        {
            if (!field[Literals.Self].As<Audio>(out var self))
                throw new ArgumentError();

            return new Int(self.Value.PrimaryAudioStream?.Channels ?? 0);
        }, []));

        field.Set("convert", new NativeFun("convert", 2, field => 
        {
            if (!field[Literals.Self].As<Audio>(out var self) || 
                !field["format"].As<Str>(out var format) || 
                !field["output"].As<Str>(out var output))
                throw new ArgumentError();

            var outputFile = $"{output.Value}.{format.Value}";
            FFMpegArguments
                .FromFileInput(self.Path.Value)
                .OutputToFile(outputFile, overwrite: true)
                .ProcessSynchronously();

            return new Str(outputFile);
        }, [("format", null!), ("output", null!)]));

        field.Set("set_volume", new NativeFun("set_volume", 2, field => 
        {
            if (!field[Literals.Self].As<Audio>(out var self) || 
                !field["output"].As<Str>(out var output) || 
                !field["volume"].As<Float>(out var volume))
                throw new ArgumentError();

            var outputFile = $"{output.Value}.mp3";
            FFMpegArguments
                .FromFileInput(self.Path.Value)
                .OutputToFile(outputFile, overwrite: true, options => options
                    .WithCustomArgument($"volume={volume.Value}"))
                .ProcessSynchronously();

            return new Str(outputFile);
        }, [("output", null!), ("volume", null!)]));

        field.Set("trim", new NativeFun("trim", 3, field => 
        {
            if (!field[Literals.Self].As<Audio>(out var self) || 
                !field["output"].As<Str>(out var output) || 
                !field["start"].As<Float>(out var start) || 
                !field["duration"].As<Float>(out var duration))
                throw new ArgumentError();

            var outputFile = $"{output.Value}.mp3";
            FFMpegArguments
                .FromFileInput(self.Path.Value)
                .OutputToFile(outputFile, overwrite: true, options => options
                    .WithCustomArgument($"-ss {start.Value} -t {duration.Value}"))
                .ProcessSynchronously();

            return new Str(outputFile);
        }, [("output", null!), ("start", null!), ("duration", null!)]));

        field.Set("merge", new NativeFun("merge", 2, field => 
        {
            if (!field["files"].As<List>(out var files) || 
                !field["output"].As<Str>(out var output))
                throw new ArgumentError();

            var inputFiles = files.Select(file => file.CStr().Value).ToArray();
            var outputFile = $"{output.Value}.mp3";

            FFMpeg.Join(outputFile, inputFiles);

            return new Str(outputFile);
        }, [("files", null!), ("output", null!)]));

        field.Set("extract_from_video", new NativeFun("extract_from_video", 2, field => 
        {
            if (!field["video_path"].As<Str>(out var videoPath) || 
                !field["output"].As<Str>(out var output))
                throw new ArgumentError();

            var outputFile = $"{output.Value}.mp3";
            FFMpegArguments
                .FromFileInput(videoPath.Value)
                .OutputToFile(outputFile, overwrite: true, options => options
                    .WithAudioCodec("mp3"))
                .ProcessSynchronously();

            return new Str(outputFile);
        }, [("video_path", null!), ("output", null!)]));
    }

    public override Obj Init(Collections.Tuple args, Field field) => throw new ClassError("cannot be created. use audio.load");

    public Obj Static()
    {
        Obj audio = new(ClassName);
        audio.field.Set("load", new NativeFun("load", 1, field => 
        {
            if (!field["path"].As<Str>(out var path))
                throw new ArgumentError();
            
            return new Audio(FFProbe.Analyse(path.Value));
       
        }, [("path", null!)]));

        return audio;
    }
}
