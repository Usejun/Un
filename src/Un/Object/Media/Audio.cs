using FFMpegCore;
using Un.Object;
using Un.Object.Function;
using Un.Object.Primitive;
using Un.Object.Collections;

namespace Un.Object.Media;

public class Audio(IMediaAnalysis value) : Ref<IMediaAnalysis>(value, "audio"), IPack
{
    public Str Path { get; set; }

    public override Obj Init(Tup args) => new Err("cannot be created. use audio.load");

    public override Attributes GetOriginal() => new()
    {
        { "duration", new NFn()
            {
                Name = "duration",
                Args = [],
                Func = (args) =>
                {
                    if (!args["self"].As<Audio>(out var self))
                        return new Err("invalid argument: self");

                    return new Float(self.Value.Duration.TotalSeconds);
                }
            }
        },
        { "bitrate", new NFn()
            {
                Name = "bitrate",
                Args = [],
                Func = (args) =>
                {
                    if (!args["self"].As<Audio>(out var self))
                        return new Err("invalid argument: self");

                    return new Int(self.Value.PrimaryAudioStream?.BitRate ?? 0);
                }
            }
        },
        { "sample_rate", new NFn()
            {
                Name = "sample_rate",
                Args = [],
                Func = (args) =>
                {
                    if (!args["self"].As<Audio>(out var self))
                        return new Err("invalid argument: self");

                    return new Int(self.Value.PrimaryAudioStream.SampleRateHz);
                }
            }
        },
        { "channels", new NFn()
            {
                Name = "channels",
                Args = [],
                Func = (args) =>
                {
                    if (!args["self"].As<Audio>(out var self))
                        return new Err("invalid argument: self");

                    return new Int(self.Value.PrimaryAudioStream?.Channels ?? 0);
                }
            }
        },
        { "save", new NFn()
            {
                Name = "save",
                Args = [
                    new Arg("output") { IsEssential = true },
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Audio>(out var self))
                        return new Err("invalid argument: self");

                    if (!args["output"].As<Str>(out var output))
                        return new Err("invalid argument: output");

                    var outputFile = output.Value[^4..] switch
                    {
                        ".mp3" or ".wav" => output.Value,
                        _ => output.Value + ".mp3"
                    };

                    if (!FFMpegArguments
                        .FromFileInput(self.Path.Value)
                        .OutputToFile(outputFile, overwrite: true)
                        .ProcessSynchronously())
                        File.Create(outputFile);

                    return new Audio(FFProbe.Analyse(outputFile))
                    {
                        Path = new(outputFile)
                    };
                }
            }
        },
        { "set_volume", new NFn()
            {
                Name = "set_volume",
                Args = [
                    new Arg("output") { IsEssential = true },
                    new Arg("volume") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Audio>(out var self))
                        return new Err("invalid argument: self");

                    if (!args["output"].As<Str>(out var output))
                        return new Err("invalid argument: output");

                    if (!args["volume"].As<Float, Int>(out var volume))
                        return new Err("invalid argument: volume");

                    var outputFile = output.Value[^4..] switch
                    {
                        ".mp3" or ".wav" => output.Value,
                        _ => output.Value + ".mp3"
                    };

                    if (FFMpegArguments
                        .FromFileInput(self.Path.Value)
                        .OutputToFile(outputFile, overwrite: true, options => options
                            .WithCustomArgument($"volume={volume.ToFloat().As<Float>().Value}"))
                        .ProcessSynchronously())
                        File.Create(outputFile);

                    return new Audio(FFProbe.Analyse(outputFile))
                    {
                        Path = new(outputFile)
                    };
                }
            }
        },
        { "trim", new NFn()
            {
                Name = "trim",
                Args = [
                    new Arg("output") { IsEssential = true },
                    new Arg("start") { IsEssential = true },
                    new Arg("duration") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Audio>(out var self))
                        return new Err("invalid argument: self");

                    if (!args["output"].As<Str>(out var output))
                        return new Err("invalid argument: output");

                    if (!args["start"].As<Float, Int>(out var start))
                        return new Err("invalid argument: start");

                    if (!args["duration"].As<Float, Int>(out var duration))
                        return new Err("invalid argument: duration");

                    var outputFile = output.Value[^4..] switch
                    {
                        ".mp3" or ".wav" => output.Value,
                        _ => output.Value + ".mp3"
                    };

                    if (FFMpegArguments
                        .FromFileInput(self.Path.Value)
                        .OutputToFile(outputFile, overwrite: true, options => options
                            .WithCustomArgument($"-ss {start.ToFloat().As<Float>().Value} -t {duration.ToFloat().As<Float>().Value}"))
                        .ProcessSynchronously())
                        File.Create(outputFile);

                    return new Audio(FFProbe.Analyse(outputFile))
                    {
                        Path = new(outputFile)
                    };
                }
            }
        },
    };

    public Attributes GetOriginalMethods() => new()
    {
        { "load", new NFn()
            {
                Name = "load",
                Args = [
                    new Arg("path") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["path"].As<Str>(out var path))
                        return new Err("invalid argument: path");

                    return new Audio(FFProbe.Analyse(path.Value))
                    {
                        Path = path
                    };
                }
            }
        },
        { "from_video", new NFn()
            {
                Name = "from_video",
                Args = [
                    new Arg("video") { IsEssential = true },
                ],
                Func = (args) =>
                {
                    if (!args["video"].As<Video, Str>(out var video))
                        return new Err("invalid argument: video");

                    var output = System.IO.Path.GetTempFileName() + ".mp3";

                    if (FFMpegArguments
                        .FromFileInput(video switch
                        {
                            Video v => v.Path.Value,
                            Str s => s.Value[^4..] switch
                            {
                                ".mp3" => s.Value,
                                _ => s.Value + ".mp3"
                            },
                            _ => throw new Panic("invalid argument: video")
                        })
                        .OutputToFile(output, overwrite: true, options => options
                            .WithAudioCodec("mp3"))
                        .ProcessSynchronously())
                        File.Create(output);

                    return new Audio(FFProbe.Analyse(output))
                    {
                        Path = new(output)
                    };
                }
            }
        },      
        { "merge", new NFn()
            {
                Name = "merge",
                Args = [
                    new Arg("files") { IsEssential = true },
                    new Arg("output") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["files"].As<List>(out var files))
                        return new Err("invalid argument: files");

                    if (!args["output"].As<Str>(out var output))
                        return new Err("invalid argument: output");

                    var inputFiles = files.Select(file => file.ToStr().As<Str>().Value).ToArray();
                    var outputFile = $"{output.Value}.mp3";

                    if (!FFMpeg.Join(outputFile, inputFiles))                    
                        File.Create(outputFile);
                        
                    return new Audio(FFProbe.Analyse(outputFile))
                    {
                        Path = new(outputFile)
                    };
                }
            }
        },  
    };

    public Attributes GetOriginalMembers() => [];
}
