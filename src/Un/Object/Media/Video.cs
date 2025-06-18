using SkiaSharp;
using FFMpegCore;
using Un.Object;
using Un.Object.Function;
using Un.Object.Primitive;
using Un.Object.Collections;

namespace Un.Object.Media;

public class Video(IMediaAnalysis value) : Ref<IMediaAnalysis>(value, "video"), IPack
{
    public Str Path { get; set; }

    public override Obj Init(Tup args) => new Err("cannot be created. use video.new or video.load");

    public override int GetHashCode() => Value.GetHashCode();

    public override Obj Copy() => this;

    public override Attributes GetOriginal() => new()
    {
        { "duration", new NFn()
            {
                Name = "duration",
                Args = [
                        new Arg("self") { IsEssential = true }
                    ],
                Func = (args) =>
                {
                    if (!args["self"].As<Video>(out var self))
                        return new Err("invalid argument: self");

                    return new Float(self.Value.Duration.TotalSeconds);
                }
            }
        },
        { "width", new NFn()
            {
                Name = "width",
                Args = [
                        new Arg("self") { IsEssential = true }
                    ],
                Func = (args) =>
                {
                    if (!args["self"].As<Video>(out var self))
                        return new Err("invalid argument: self");

                    return new Int(self.Value.PrimaryVideoStream.Width);
                }
            }
        },
        { "height", new NFn()
            {
                Name = "height",
                Args = [
                        new Arg("self") { IsEssential = true }
                    ],
                Func = (args) =>
                {
                    if (!args["self"].As<Video>(out var self))
                        return new Err("invalid argument: self");

                    return new Int(self.Value.PrimaryVideoStream.Height);
                }
            }
        },
        { "resize", new NFn()
            {
                Name = "resize",
                Args = [
                        new Arg("self") { IsEssential = true },
                        new Arg("width") { IsEssential = true },
                        new Arg("height") { IsEssential = true },
                        new Arg("output") { IsEssential = true }
                    ],
                Func = (args) =>
                {
                    if (!args["self"].As<Video>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["width"].As<Int, Float>(out var width))
                        return new Err("invalid argument: width");
                    if (!args["height"].As<Int, Float>(out var height))
                        return new Err("invalid argument: height");
                    if (!args["output"].As<Str>(out var output))
                        return new Err("invalid argument: output");

                    var inputPath = self.Path.Value;
                    FFMpegArguments
                        .FromFileInput(inputPath)
                        .OutputToFile(output.Value, overwrite: true, options => options
                            .Resize((int)width.ToInt().As<Int>().Value, (int)height.ToInt().As<Int>().Value)
                            .WithVideoCodec("libx264")
                            .WithAudioCodec("aac")
                            .WithFramerate(self.Value.PrimaryVideoStream.FrameRate))
                        .ProcessSynchronously();

                    return new Video(FFProbe.Analyse(output.Value))
                    {
                        Path = output
                    };
                }
            }
        },
        { "fps", new NFn()
            {
                Name = "fps",
                Args = [],
                Func = (args) =>
                {
                    if (!args["self"].As<Video>(out var self))
                        return new Err("invalid argument: self");

                    return new Float(self.Value.PrimaryVideoStream.FrameRate);
                }
            }
        },
        { "codec", new NFn()
            {
                Name = "codec",
                Args = [],
                Func = (args) =>
                {
                    if (!args["self"].As<Video>(out var self))
                        return new Err("invalid argument: self");

                    return new Str(self.Value.PrimaryVideoStream?.CodecName);
                }
            }
        },
        { "save", new NFn()
            {
                Name = "save",
                Args = [
                    new Arg("output") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Video>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["output"].As<Str>(out var output))
                        return new Err("invalid argument: output");

                    var inputPath = self.Path.Value;
                    var outputPath = output.Value[^4..] switch
                    {
                        ".mp4" or ".avi" => output.Value,
                        _ => output.Value + ".mp4"
                    };

                    FFMpegArguments
                        .FromFileInput(inputPath)
                        .OutputToFile(output.Value, overwrite: true, options => options
                            .WithCustomArgument("-c:v libx264 -preset slow -crf 23 -c:a aac -b:a 128k"))
                        .ProcessSynchronously();

                    return new Video(FFProbe.Analyse(output.Value))
                    {
                        Path = output
                    };
                }
            }
        },
        { "thumbnail", new NFn()
            {
                Name = "thumbnail",
                Args = [
                    new Arg("time") { IsEssential = true },
                    new Arg("format") { IsOptional = true, DefaultValue = new Str("jpg")}
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Video>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["time"].As<Float, Int>(out var time))
                        return new Err("invalid argument: time");
                    if (!args["format"].As<Str>(out var format))
                        return new Err("invalid argument: format");

                    var output = System.IO.Path.GetTempFileName() + format.Value;
                    var input = self.Path;

                    FFMpegArguments
                        .FromFileInput(input.Value)
                        .OutputToFile(output, overwrite: true, options => options
                            .WithCustomArgument($"-vf \"select='gte(n\\,{time.ToFloat().As<Float>().Value * self.Value.PrimaryVideoStream.FrameRate})'\" -frames:v 1"))
                        .ProcessSynchronously();

                    using var stream = new SKFileStream(output);
                    return new Image(SKBitmap.Decode(stream) ?? throw new Panic("failed to load image"));
                }
            }
        },
        { "trim", new NFn()
            {
                Name = "trim",
                Args = [
                    new Arg("start") { IsEssential = true },
                    new Arg("duration") { IsEssential = true },
                    new Arg("output") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Video>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["start"].As<Int, Float>(out var start))
                        return new Err("invalid argument: start");
                    if (!args["duration"].As<Int, Float>(out var duration))
                        return new Err("invalid argument: duration");
                    if (!args["output"].As<Str>(out var output))
                        return new Err("invalid argument: output");

                    var inputPath = self.Path;
                    var outputPath = output.Value[^4..] switch
                    {
                        ".mp4" or ".avi" => output.Value,
                        _ => output.Value + ".mp4"
                    };

                    FFMpegArguments
                        .FromFileInput(inputPath.Value, true, opt => opt
                            .Seek(TimeSpan.FromSeconds(start.ToFloat().As<Float>().Value)))
                        .OutputToFile(outputPath, overwrite: true, opt => opt
                            .WithDuration(TimeSpan.FromSeconds(duration.ToFloat().As<Float>().Value))
                            .WithAudioCodec("aac"))
                        .ProcessSynchronously();

                    return new Video(FFProbe.Analyse(outputPath))
                    {
                        Path = new(outputPath)
                    };
                }
            }
        },
        { "extract_audio", new NFn()
            {
                Name = "extract_audio",
                Args = [ new Arg("output") { IsEssential = true } ],
                Func = (args) =>
                {
                    if (!args["self"].As<Video>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["output"].As<Str>(out var output))
                        return new Err("invalid argument: output");

                    var inputPath = self.Path;
                    var outputPath = output.Value[^4..] switch
                    {
                        ".mp3" => output.Value,
                        _ => output.Value + ".mp3"
                    };

                    FFMpegArguments
                        .FromFileInput(inputPath.Value)
                        .OutputToFile(output.Value, overwrite: true, options => options
                            .WithCustomArgument("-vn -acodec copy"))
                        .ProcessSynchronously();

                    return new Audio(FFProbe.Analyse(outputPath))
                    {
                        Path = new(outputPath)
                    };
                }
            }
        },
        { "replace_audio", new NFn()
            {
                Name = "replace_audio",
                Args = [
                    new Arg("audio") { IsEssential = true },
                    new Arg("output") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Video>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["audio"].As<Str, Audio>(out var audio))
                        return new Err("invalid argument: audio");
                    if (!args["output"].As<Str>(out var output))
                        return new Err("invalid argument: output");

                    var inputPath = self.Path;
                    var outputPath = output.Value[^4..] switch
                    {
                        ".mp3" => output.Value,
                        _ => output.Value + ".mp3"
                    };

                    FFMpegArguments
                        .FromFileInput(inputPath.Value)
                        .AddFileInput(audio switch
                        {
                            Str s => s.Value,
                            Audio a => a.Path.Value,
                            _ => throw new Panic("invalid argument: audio")
                        })
                        .OutputToFile(outputPath, overwrite: true, options => options
                            .WithCustomArgument("-map 0:v:0 -map 1:a:0 -c:v copy -c:a aac"))
                        .ProcessSynchronously();


                    return new Video(FFProbe.Analyse(outputPath))
                    {
                        Path = new(outputPath)
                    };
                }
            }
        },
        { "key_frames", new NFn()
            {
                Name = "key_frames",
                Args = [],
                Func = (args) =>
                {
                    if (!args["self"].As<Video>(out var self))
                        return new Err("invalid argument: self");

                    var tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
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

                    Directory.Delete(tempDir, true);

                    return images;

                }
            }
        },
    };

    public Attributes GetOriginalMembers() => [];

    public Attributes GetOriginalMethods() => new()
    {
        { "load", new NFn()
            {
                Name = "load",
                Args = [ new Arg("path") { IsEssential = true} ],
                Func = (args) =>
                {
                    if (!args["path"].As<Str>(out var path))
                        return new Err("invalid arguments: path");

                    var fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, path.Value));

                    if (!File.Exists(fullPath))
                        return new Err("A file that doesn't exist.");

                    var analysis = FFProbe.Analyse(fullPath);

                    return new Video(analysis)
                    {
                        Path = new Str(fullPath)
                    };
                }
            }
        },
        { "concat", new NFn()
            {
                Name = "concat",
                Args = [
                    new Arg("videos") { IsEssential = true },
                    new Arg("output") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["videos"].As<List>(out var videos))
                        return new Err("invalid argument: videos");
                    if (!args["output"].As<Str>(out var output))
                        return new Err("invalid argument: output");

                    var tempFile = System.IO.Path.GetTempFileName();
                    try
                    {
                        using (var writer = new StreamWriter(tempFile))
                        {
                            foreach (var video in videos)
                            {
                                var escapedPath = video.As<Str, Video>() switch
                                {
                                    Str s => s.Value.Replace("'", "'\\''"),
                                    Video v => v.Path.Value.Replace("'", "'\\''"),
                                    _ => throw new Panic("invalid argument: videos")
                                };
                                writer.WriteLine($"file '{escapedPath}'");
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
                    }
                    finally
                    {
                        File.Delete(tempFile);
                    }

                }
            }
        },
    };
    
}


