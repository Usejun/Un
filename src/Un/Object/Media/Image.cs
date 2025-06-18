using SkiaSharp;
using Un.Object;
using Un.Object.Function;
using Un.Object.Primitive;
using Un.Object.Collections;
using System.Text;

namespace Un.Object.Media;

public class Image(SKBitmap value) : Ref<SKBitmap>(value ?? new SKBitmap(), "image"), IPack
{
    public override Obj Init(Tup args) => new Err("cannot be created. use image.new or image.load");

    public override Obj Clone() => new Image(Value.Copy());

    public override Obj Copy() => this;

    public override Attributes GetOriginal() => new()
    {
        { "save", new NFn()
            {
                Name = "save",
                Args = [
                    new Arg("path") { IsEssential = true },
                    new Arg("format") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["path"].As<Str>(out var path))
                        return new Err("invalid argument: path");
                    if (!args["format"].As<Str>(out var format))
                        return new Err("invalid argument: format");

                    var imgFormat = format.Value.ToLower() switch
                    {
                        "png" => SKEncodedImageFormat.Png,
                        "jpg" or "jpeg" => SKEncodedImageFormat.Jpeg,
                        "bmp" => SKEncodedImageFormat.Bmp,
                        "ico" => SKEncodedImageFormat.Ico,
                        _ => throw new Panic("unsupported format")
                    };

                    using var image = SKImage.FromBitmap(self.Value);
                    using var data = image.Encode(imgFormat, 100);
                    using var file = File.OpenWrite(path.Value);
                    data.SaveTo(file);

                    return None;
                }
            }
        },
        { "get_pixel", new NFn()
            {
                Name = "get_pixel",
                Args = [
                    new Arg("x") { IsEssential = true },
                    new Arg("y") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["x"].As<Int, Float>(out var x))
                        return new Err("invalid argument: x");
                    if (!args["y"].As<Int, Float>(out var y))
                        return new Err("invalid argument: y");

                    var color = self.Value.GetPixel((int)x.ToInt().As<Int>().Value, (int)y.ToInt().As<Int>().Value);
                    return new Tup([new Int(color.Red), new Int(color.Green), new Int(color.Blue), new Int(color.Alpha)], ["red", "green", "blue", "alpha"]);
                }
            }
        },
        { "set_pixel", new NFn()
            {
                Name = "set_pixel",
                Args = [
                    new Arg("x") { IsEssential = true },
                    new Arg("y") { IsEssential = true },
                    new Arg("r") { IsEssential = true },
                    new Arg("g") { IsEssential = true },
                    new Arg("b") { IsEssential = true },
                    new Arg("a") { IsOptional = true, DefaultValue = new Int(255) }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["x"].As<Int, Float>(out var x))
                        return new Err("invalid argument: x");
                    if (!args["y"].As<Int, Float>(out var y))
                        return new Err("invalid argument: y");
                    if (!args["r"].As<Int, Float>(out var r))
                        return new Err("invalid argument: r");
                    if (!args["g"].As<Int, Float>(out var g))
                        return new Err("invalid argument: g");
                    if (!args["b"].As<Int, Float>(out var b))
                        return new Err("invalid argument: b");
                    if (!args["a"].As<Int, Float>(out var a))
                        return new Err("invalid argument: a");

                    self.Value.SetPixel((int)x.ToInt().As<Int>().Value, (int)y.ToInt().As<Int>().Value,
                            new SKColor((byte)r.ToInt().As<Int>().Value, (byte)g.ToInt().As<Int>().Value, (byte)b.ToInt().As<Int>().Value, (byte)a.ToInt().As<Int>().Value));
                    return None;
                }
            }
        },
        { "resize", new NFn()
            {
                Name = "resize",
                Args = [
                    new Arg("width") { IsEssential = true },
                    new Arg("height") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["width"].As<Int, Float>(out var width))
                        return new Err("invalid argument: width");
                    if (!args["height"].As<Int, Float>(out var height))
                        return new Err("invalid argument: height");

                    if (width.ToInt().As<Int>().Value <= 0)
                        return new Err("width is non-zero");
                    if (height.ToInt().As<Int>().Value <= 0)
                        return new Err("height is non-zero");

                    var resized = self.Value.Resize(new SKImageInfo((int)width.ToInt().As<Int>().Value, (int)height.ToInt().As<Int>().Value),
                                                    new SKSamplingOptions(SKCubicResampler.Mitchell));
                    return new Image(resized);
                }
            }
        },
        { "rotate", new NFn()
            {
                Name = "rotate",
                Args = [
                    new Arg("angle") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["angle"].As<Int, Float>(out var angle))
                        return new Err("invalid argument: angle");

                    var matrix = SKMatrix.CreateRotationDegrees((float)angle.ToFloat().As<Float>().Value, self.Value.Width / 2, self.Value.Height / 2);
                    var rotated = new SKBitmap(self.Value.Width, self.Value.Height);
                    using var canvas = new SKCanvas(rotated);

                    canvas.Clear(SKColors.Transparent);
                    canvas.SetMatrix(matrix);
                    canvas.DrawBitmap(self.Value, 0, 0);

                    return new Image(rotated);
                }
            }
        },
        { "draw_text", new NFn()
            {
                Name = "draw_text",
                Args = [
                    new Arg("text") { IsEssential = true },
                    new Arg("x") { IsEssential = true },
                    new Arg("y") { IsEssential = true },
                    new Arg("size") { IsEssential = true },
                    new Arg("color") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["text"].As<Str>(out var text))
                        return new Err("invalid argument: text");
                    if (!args["x"].As<Int, Float>(out var x))
                        return new Err("invalid argument: x");
                    if (!args["y"].As<Int, Float>(out var y))
                        return new Err("invalid argument: y");
                    if (!args["size"].As<Int, Float>(out var size))
                        return new Err("invalid argument: size");
                    if (!args["color"].As<Str>(out var color))
                        return new Err("invalid argument: color");

                    using var canvas = new SKCanvas(self.Value);
                    using var font = new SKFont(SKTypeface.Default, size.ToInt().As<Int>().Value);
                    using var paint = new SKPaint()
                    {
                        Color = SKColor.Parse(color.Value),
                        IsAntialias = true
                    };
                    canvas.DrawText(text.Value, x.ToInt().As<Int>().Value, y.ToInt().As<Int>().Value, font, paint);
                    return None;
                }
            }
        },
        { "draw_rect", new NFn()
            {
                Name = "draw_rect",
                Args = [
                    new Arg("x") { IsEssential = true },
                    new Arg("y") { IsEssential = true },
                    new Arg("width") { IsEssential = true },
                    new Arg("height") { IsEssential = true },
                    new Arg("color") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["x"].As<Int, Float>(out var x))
                        return new Err("invalid argument: x");
                    if (!args["y"].As<Int, Float>(out var y))
                        return new Err("invalid argument: y");
                    if (!args["width"].As<Int, Float>(out var width))
                        return new Err("invalid argument: width");
                    if (!args["height"].As<Int, Float>(out var height))
                        return new Err("invalid argument: height");
                    if (!args["color"].As<Str>(out var color))
                        return new Err("invalid argument: color");

                    using var canvas = new SKCanvas(self.Value);
                    using var paint = new SKPaint
                    {
                        Color = SKColor.Parse(color.Value),
                        Style = SKPaintStyle.Fill
                    };
                    canvas.DrawRect(x.ToInt().As<Int>().Value, y.ToInt().As<Int>().Value,
                                    width.ToInt().As<Int>().Value, height.ToInt().As<Int>().Value, paint);
                    return None;
                }
            }
        },
        { "draw_line", new NFn()
            {
                Name = "draw_line",
                Args = [
                    new Arg("x1") { IsEssential = true },
                    new Arg("y1") { IsEssential = true },
                    new Arg("x2") { IsEssential = true },
                    new Arg("y2") { IsEssential = true },
                    new Arg("thickness") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["x1"].As<Int, Float>(out var x1))
                        return new Err("invalid argument: x1");
                    if (!args["y1"].As<Int, Float>(out var y1))
                        return new Err("invalid argument: y1");
                    if (!args["x2"].As<Int, Float>(out var x2))
                        return new Err("invalid argument: x2");
                    if (!args["y2"].As<Int, Float>(out var y2))
                        return new Err("invalid argument: y2");
                    if (!args["thickness"].As<Int, Float>(out var thickness))
                        return new Err("invalid argument: thickness");

                    using var canvas = new SKCanvas(self.Value);
                    using var paint = new SKPaint { Color = SKColors.Black, StrokeWidth = (float)thickness.ToFloat().As<Float>().Value };
                    canvas.DrawLine(x1.ToInt().As<Int>().Value, y1.ToInt().As<Int>().Value, x2.ToInt().As<Int>().Value, y2.ToInt().As<Int>().Value, paint);
                    return None;
                }
            }
        },
        { "draw_circle", new NFn()
            {
                Name = "draw_circle",
                Args = [
                    new Arg("x") { IsEssential = true },
                    new Arg("y") { IsEssential = true },
                    new Arg("radius") { IsEssential = true },
                    new Arg("thickness") { IsEssential = true },
                    new Arg("fill") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self) || self.Value == null)
                        return new Err("invalid argument: self");
                    if (!args["x"].As<Int, Float>(out var x))
                        return new Err("invalid argument: x");
                    if (!args["y"].As<Int, Float>(out var y))
                        return new Err("invalid argument: y");
                    if (!args["radius"].As<Int, Float>(out var radius))
                        return new Err("invalid argument: radius");
                    if (!args["thickness"].As<Int, Float>(out var thickness))
                        return new Err("invalid argument: thickness");
                    if (!args["fill"].As<Bool>(out var fill))
                        return new Err("invalid argument: fill");

                    using var canvas = new SKCanvas(self.Value);
                    using var paint = new SKPaint { Color = SKColors.Red, StrokeWidth = (float)thickness.ToFloat().As<Float>().Value };
                    paint.Style = fill.Value ? SKPaintStyle.Fill : SKPaintStyle.Stroke;
                    canvas.DrawCircle(x.ToInt().As<Int>().Value, y.ToInt().As<Int>().Value, radius.ToInt().As<Int>().Value, paint);
                    return None;
                }
            }
        },
        { "apply_grayscale", new NFn()
            {
                Name = "apply_grayscale",
                Args = [],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self))
                        return new Err("invalid argument: self");

                    var grayBitmap = new SKBitmap(self.Value.Width, self.Value.Height);
                    using var canvas = new SKCanvas(grayBitmap);
                    using var paint = new SKPaint
                    {
                        ColorFilter = SKColorFilter.CreateColorMatrix(
                        [
                            0.3f, 0.59f, 0.11f, 0, 0,
                            0.3f, 0.59f, 0.11f, 0, 0,
                            0.3f, 0.59f, 0.11f, 0, 0,
                            0, 0, 0, 1, 0
                        ])
                    };

                    canvas.DrawBitmap(self.Value, 0, 0, paint);
                    return new Image(grayBitmap);
                }
            }
        },
        { "apply_blur", new NFn()
            {
                Name = "apply_blur",
                Args = [
                    new Arg("radius") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["radius"].As<Int, Float>(out var radius))
                        return new Err("invalid argument: radius");

                    using var surface = SKSurface.Create(self.Value.Info);
                    using var canvas = surface.Canvas;
                    using var paint = new SKPaint
                    {
                        ImageFilter = SKImageFilter.CreateBlur((float)radius.ToFloat().As<Float>().Value, (float)radius.ToFloat().As<Float>().Value)
                    };
                    canvas.DrawBitmap(self.Value, 0, 0, paint);
                    self.Value = SKBitmap.Decode(surface.Snapshot().Encode());
                    return None;
                }
            }
        },
        { "crop", new NFn()
            {
                Name = "crop",
                Args = [
                    new Arg("x") { IsEssential = true },
                    new Arg("y") { IsEssential = true },
                    new Arg("width") { IsEssential = true },
                    new Arg("height") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self) || self.Value == null)
                        return new Err("invalid argument: self");
                    if (!args["x"].As<Int, Float>(out var x))
                        return new Err("invalid argument: x");
                    if (!args["y"].As<Int, Float>(out var y))
                        return new Err("invalid argument: y");
                    if (!args["width"].As<Int, Float>(out var width) || width.ToInt().As<Int>().Value <= 0)
                        return new Err("invalid argument: width");
                    if (!args["height"].As<Int, Float>(out var height) || height.ToInt().As<Int>().Value <= 0)
                        return new Err("invalid argument: height");

                    var rect = new SKRectI((int)x.ToInt().As<Int>().Value, (int)y.ToInt().As<Int>().Value,
                                           (int)(x.ToInt().As<Int>().Value + width.ToInt().As<Int>().Value),
                                           (int)(y.ToInt().As<Int>().Value + height.ToInt().As<Int>().Value));
                    var copyBitmap = self.Value.Copy();
                    var croppedBitmap = self.Value.ExtractSubset(copyBitmap, rect);
                    return new Image(copyBitmap);
                }
            }
        },
        { "adjust_opacity", new NFn()
            {
                Name = "adjust_opacity",
                Args = [new Arg("opacity") { IsEssential = true }],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["opacity"].As<Int, Float>(out var opacity))
                        return new Err("invalid argument: opacity");

                    var adjustedBitmap = new SKBitmap(self.Value.Width, self.Value.Height);
                    using var canvas = new SKCanvas(adjustedBitmap);
                    using var paint = new SKPaint
                    {
                        ColorFilter = SKColorFilter.CreateBlendMode(SKColors.Black.WithAlpha((byte)(255 * (1 - (opacity.ToFloat().As<Float>().Value % 1f)))), SKBlendMode.DstIn)
                    };

                    canvas.DrawBitmap(self.Value, 0, 0, paint);
                    return new Image(adjustedBitmap);
                }
            }
        },
        { "apply_color_shift", new NFn()
            {
                Name = "apply_color_shift",
                Args = [
                    new Arg("r") { IsEssential = true },
                    new Arg("g") { IsEssential = true },
                    new Arg("b") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["r"].As<Int, Float>(out var r))
                        return new Err("invalid argument: r");
                    if (!args["g"].As<Int, Float>(out var g))
                        return new Err("invalid argument: g");
                    if (!args["b"].As<Int, Float>(out var b))
                        return new Err("invalid argument: r");

                    var colorMatrix = new float[]
                    {
                        1, 0, 0, 0, (float)r.ToFloat().As<Float>().Value, 
                        0, 1, 0, 0, (float)g.ToFloat().As<Float>().Value,
                        0, 0, 1, 0, (float)b.ToFloat().As<Float>().Value,
                        0, 0, 0, 1, 0
                    };
                    using var paint = new SKPaint
                    {
                        ColorFilter = SKColorFilter.CreateColorMatrix(colorMatrix)
                    };
                    var shiftedBitmap = new SKBitmap(self.Value.Width, self.Value.Height);
                    using (var canvas = new SKCanvas(shiftedBitmap))
                    {
                        canvas.DrawBitmap(self.Value, 0, 0, paint);
                    }
                    return new Image(shiftedBitmap);
                }
            }
        },
        { "compare", new NFn()
            {
                Name = "compare",
                Args = [new Arg("other") { IsEssential = true }],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self))
                        return  new Err("invalid argument: self");
                    if (!args["other"].As<Image>(out var other))
                        return  new Err("invalid argument: other");

                    var diff = 0;
                    for (int y = 0; y < self.Value.Height; y++)
                    {
                        for (int x = 0; x < self.Value.Width; x++)
                        {
                            var selfPixel = self.Value.GetPixel(x, y);
                            var otherPixel = other.Value.GetPixel(x, y);
                            diff += (selfPixel.Red != otherPixel.Red) ? 1 : 0;
                            diff += (selfPixel.Green != otherPixel.Green) ? 1 : 0;
                            diff += (selfPixel.Blue != otherPixel.Blue) ? 1 : 0;
                        }
                    }

                    return new Int(diff);
                }
            }
        },
        { "to_ascii", new NFn()
            {
                Name = "to_ascii",
                Args = [],
                Func = (args) =>
                {
                    if (!args["self"].As<Image>(out var self))
                        return  new Err("invalid argument: self");

                    string ascii = "@%#*+=-:. ";

                    var bitmap = self.Value;
                    var sb = new StringBuilder();

                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            var color = bitmap.GetPixel(x, y);
                            int brightness = (color.Red + color.Green + color.Blue) / 3;
                            int index = brightness * (ascii.Length - 1) / 255;
                            sb.Append(ascii[index]);
                        }
                        sb.AppendLine();
                    }

                    return new Str(sb.ToString());
                }
            }
        }
    };

    public Attributes GetOriginalMembers() => new();

    public Attributes GetOriginalMethods() => new()
    {
        { "load", new NFn()
            {
                Name = "load",
                Args = [ new Arg("path") { IsEssential = true} ],
                Func = (args) =>
                {
                    if (!args["path"].As<Str>(out var path))
                        new Err("invalid arguments: path");

                    var fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, path.Value));

                    if (!File.Exists(fullPath))
                        new Err("A file that doesn't exist.");

                    using var stream = new SKFileStream(fullPath);
                    var bitmap = SKBitmap.Decode(stream);
                    return new Image(bitmap ?? throw new Panic("failed to load image"));
                }
            }
        },
        { "new", new NFn()
            {
                Name = "new",
                Args = [
                    new Arg("width") { IsEssential = true },
                    new Arg("height") { IsEssential = true },
                ],
                Func = (args) =>
                {
                    if (!args["width"].As<Int, Float>(out var width))
                        return new Err("invalid arguments: width");
                    if (!args["height"].As<Int, Float>(out var height))
                        return new Err("invalid arguments: height");

                    var bitmap = new SKBitmap((int)width.ToInt().As<Int>().Value, (int)height.ToInt().As<Int>().Value);

                    return new Image(bitmap);
                }
            }
        },
    };
}
