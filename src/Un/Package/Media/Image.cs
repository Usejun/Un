using SkiaSharp;
using Un.Object;
using Un.Object.Function;
using Un.Object.Primitive;
using Un.Object.Collections;
using System.Text;

namespace Un.Package;

public class Image(SKBitmap value) : Ref<SKBitmap>(value ?? new SKBitmap(), "image"), IPack
{
    public override Obj Init(Tup args) => throw new Error("cannot be created. use image.new or image.load");

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
                        throw new Error("invalid argument: self");
                    if (!args["path"].As<Str>(out var path))
                        throw new Error("invalid argument: path");
                    if (!args["format"].As<Str>(out var format))
                        throw new Error("invalid argument: format");

                    var imgFormat = format.Value.ToLower() switch
                    {
                        "png" => SKEncodedImageFormat.Png,
                        "jpg" or "jpeg" => SKEncodedImageFormat.Jpeg,
                        "bmp" => SKEncodedImageFormat.Bmp,
                        "ico" => SKEncodedImageFormat.Ico,
                        _ => throw new Error("unsupported format")
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
                        throw new Error("invalid argument: self");
                    if (!args["x"].As<Int, Float>(out var x))
                        throw new Error("invalid argument: x");
                    if (!args["y"].As<Int, Float>(out var y))
                        throw new Error("invalid argument: y");

                    var color = self.Value.GetPixel((int)x.ToInt().Value, (int)y.ToInt().Value);
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
                        throw new Error("invalid argument: self");
                    if (!args["x"].As<Int, Float>(out var x))
                        throw new Error("invalid argument: x");
                    if (!args["y"].As<Int, Float>(out var y))
                        throw new Error("invalid argument: y");
                    if (!args["r"].As<Int, Float>(out var r))
                        throw new Error("invalid argument: r");
                    if (!args["g"].As<Int, Float>(out var g))
                        throw new Error("invalid argument: g");
                    if (!args["b"].As<Int, Float>(out var b))
                        throw new Error("invalid argument: b");
                    if (!args["a"].As<Int, Float>(out var a))
                        throw new Error("invalid argument: a");

                    self.Value.SetPixel((int)x.ToInt().Value, (int)y.ToInt().Value,
                            new SKColor((byte)r.ToInt().Value, (byte)g.ToInt().Value, (byte)b.ToInt().Value, (byte)a.ToInt().Value));
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
                        throw new Error("invalid argument: self");
                    if (!args["width"].As<Int, Float>(out var width))
                        throw new Error("invalid argument: width");
                    if (!args["height"].As<Int, Float>(out var height))
                        throw new Error("invalid argument: height");

                    if (width.ToInt().Value <= 0)
                        throw new Error("width is non-zero");
                    if (height.ToInt().Value <= 0)
                        throw new Error("height is non-zero");

                    var resized = self.Value.Resize(new SKImageInfo((int)width.ToInt().Value, (int)height.ToInt().Value),
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
                        throw new Error("invalid argument: self");
                    if (!args["angle"].As<Int, Float>(out var angle))
                        throw new Error("invalid argument: angle");

                    var matrix = SKMatrix.CreateRotationDegrees((float)angle.ToFloat().Value, self.Value.Width / 2, self.Value.Height / 2);
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
                        throw new Error("invalid argument: self");
                    if (!args["text"].As<Str>(out var text))
                        throw new Error("invalid argument: text");
                    if (!args["x"].As<Int, Float>(out var x))
                        throw new Error("invalid argument: x");
                    if (!args["y"].As<Int, Float>(out var y))
                        throw new Error("invalid argument: y");
                    if (!args["size"].As<Int, Float>(out var size))
                        throw new Error("invalid argument: size");
                    if (!args["color"].As<Str>(out var color))
                        throw new Error("invalid argument: color");

                    using var canvas = new SKCanvas(self.Value);
                    using var font = new SKFont(SKTypeface.Default, size.ToInt().Value);
                    using var paint = new SKPaint()
                    {
                        Color = SKColor.Parse(color.Value),
                        IsAntialias = true
                    };
                    canvas.DrawText(text.Value, x.ToInt().Value, y.ToInt().Value, font, paint);
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
                        throw new Error("invalid argument: self");
                    if (!args["x"].As<Int, Float>(out var x))
                        throw new Error("invalid argument: x");
                    if (!args["y"].As<Int, Float>(out var y))
                        throw new Error("invalid argument: y");
                    if (!args["width"].As<Int, Float>(out var width))
                        throw new Error("invalid argument: width");
                    if (!args["height"].As<Int, Float>(out var height))
                        throw new Error("invalid argument: height");
                    if (!args["color"].As<Str>(out var color))
                        throw new Error("invalid argument: color");

                    using var canvas = new SKCanvas(self.Value);
                    using var paint = new SKPaint
                    {
                        Color = SKColor.Parse(color.Value),
                        Style = SKPaintStyle.Fill
                    };
                    canvas.DrawRect(x.ToInt().Value, y.ToInt().Value, width.ToInt().Value, height.ToInt().Value, paint);
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
                        throw new Error("invalid argument: self");
                    if (!args["x1"].As<Int, Float>(out var x1))
                        throw new Error("invalid argument: x1");
                    if (!args["y1"].As<Int, Float>(out var y1))
                        throw new Error("invalid argument: y1");
                    if (!args["x2"].As<Int, Float>(out var x2))
                        throw new Error("invalid argument: x2");
                    if (!args["y2"].As<Int, Float>(out var y2))
                        throw new Error("invalid argument: y2");
                    if (!args["thickness"].As<Int, Float>(out var thickness))
                        throw new Error("invalid argument: thickness");

                    using var canvas = new SKCanvas(self.Value);
                    using var paint = new SKPaint { Color = SKColors.Black, StrokeWidth = (float)thickness.ToFloat().Value };
                    canvas.DrawLine(x1.ToInt().Value, y1.ToInt().Value, x2.ToInt().Value, y2.ToInt().Value, paint);
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
                        throw new Error("invalid argument: self");
                    if (!args["x"].As<Int, Float>(out var x))
                        throw new Error("invalid argument: x");
                    if (!args["y"].As<Int, Float>(out var y))
                        throw new Error("invalid argument: y");
                    if (!args["radius"].As<Int, Float>(out var radius))
                        throw new Error("invalid argument: radius");
                    if (!args["thickness"].As<Int, Float>(out var thickness))
                        throw new Error("invalid argument: thickness");
                    if (!args["fill"].As<Bool>(out var fill))
                        throw new Error("invalid argument: fill");

                    using var canvas = new SKCanvas(self.Value);
                    using var paint = new SKPaint { Color = SKColors.Red, StrokeWidth = (float)thickness.ToFloat().Value };
                    paint.Style = fill.Value ? SKPaintStyle.Fill : SKPaintStyle.Stroke;
                    canvas.DrawCircle(x.ToInt().Value, y.ToInt().Value, radius.ToInt().Value, paint);
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
                        throw new Error("invalid argument: self");

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
                        throw new Error("invalid argument: self");
                    if (!args["radius"].As<Int, Float>(out var radius))
                        throw new Error("invalid argument: radius");

                    using var surface = SKSurface.Create(self.Value.Info);
                    using var canvas = surface.Canvas;
                    using var paint = new SKPaint
                    {
                        ImageFilter = SKImageFilter.CreateBlur((float)radius.ToFloat().Value, (float)radius.ToFloat().Value)
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
                        throw new Error("invalid argument: self");
                    if (!args["x"].As<Int, Float>(out var x))
                        throw new Error("invalid argument: x");
                    if (!args["y"].As<Int, Float>(out var y))
                        throw new Error("invalid argument: y");
                    if (!args["width"].As<Int, Float>(out var width) || width.ToInt().Value <= 0)
                        throw new Error("invalid argument: width");
                    if (!args["height"].As<Int, Float>(out var height) || height.ToInt().Value <= 0)
                        throw new Error("invalid argument: height");

                    var rect = new SKRectI((int)x.ToInt().Value, (int)y.ToInt().Value, (int)(x.ToInt().Value + width.ToInt().Value), (int)(y.ToInt().Value + height.ToInt().Value));
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
                        throw new Error("invalid argument: self");
                    if (!args["opacity"].As<Int, Float>(out var opacity))
                        throw new Error("invalid argument: opacity");

                    var adjustedBitmap = new SKBitmap(self.Value.Width, self.Value.Height);
                    using var canvas = new SKCanvas(adjustedBitmap);
                    using var paint = new SKPaint
                    {
                        ColorFilter = SKColorFilter.CreateBlendMode(SKColors.Black.WithAlpha((byte)(255 * (1 - (opacity.ToFloat().Value % 1f)))), SKBlendMode.DstIn)
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
                        throw new Error("invalid argument: self");
                    if (!args["r"].As<Int, Float>(out var r))
                        throw new Error("invalid argument: r");
                    if (!args["g"].As<Int, Float>(out var g))
                        throw new Error("invalid argument: g");
                    if (!args["b"].As<Int, Float>(out var b))
                        throw new Error("invalid argument: r");

                    var colorMatrix = new float[]
                    {
                        1, 0, 0, 0, (float)r.ToFloat().Value, 
                        0, 1, 0, 0, (float)g.ToFloat().Value,
                        0, 0, 1, 0, (float)b.ToFloat().Value,
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
                        throw new Error("invalid argument: self");
                    if (!args["other"].As<Image>(out var other))
                        throw new Error("invalid argument: other");

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
                        throw new Error("invalid argument: self");

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
                        throw new Error("invalid arguments: path");

                    var fullPath = Path.GetFullPath(Global.Path + path.Value);

                    if (!File.Exists(fullPath))
                        throw new Error("A file that doesn't exist.");

                    using var stream = new SKFileStream(fullPath);
                    var bitmap = SKBitmap.Decode(stream);
                    return new Image(bitmap ?? throw new Error("failed to load image"));
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
                        throw new Error("invalid arguments: width");
                    if (!args["height"].As<Int, Float>(out var height))
                        throw new Error("invalid arguments: height");

                    var bitmap = new SKBitmap((int)width.ToInt().Value, (int)height.ToInt().Value);

                    return new Image(bitmap);
                }
            }
        },
    };
}
