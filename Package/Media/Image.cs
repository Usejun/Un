using SkiaSharp;

namespace Un.Package;

public class Image : Ref<SKBitmap>, IStatic
{
    public Image() : base("image", null!) { }

    public Image(SKBitmap bitmap) : base("image", bitmap) {}

    public override void Init()
    {
        field.Set("save", new NativeFun("save", 2, field =>
        {
            if (!field[Literals.Self].As<Image>(out var self))
                throw new ValueError("invalid image object");
            if (!field["path"].As<Str>(out var path))
                throw new ValueError("invalid file path");
            if (!field["format"].As<Str>(out var format))
                throw new ValueError("invalid format");

            var imgFormat = format.Value.ToLower() switch
            {
                "png" => SKEncodedImageFormat.Png,
                "jpg" or "jpeg" => SKEncodedImageFormat.Jpeg,
                "bmp" => SKEncodedImageFormat.Bmp,
                "ico" => SKEncodedImageFormat.Ico,
                _ => throw new ValueError("unsupported format")
            };        

            using var image = SKImage.FromBitmap(self.Value);
            using var data = image.Encode(imgFormat, 100);
            using var file = File.OpenWrite(path.Value);
            data.SaveTo(file);

            return None;
        }, [("path", null!), ("format", null!)]));

        field.Set("get_pixel", new NativeFun("get_pixel", 2, field =>
        {
            if (!field[Literals.Self].As<Image>(out var self))
                throw new ValueError("invalid image object");
            if (!field["x"].As<Int>(out var x))
                throw new ValueError("invalid x coordinate");
            if (!field["y"].As<Int>(out var y))
                throw new ValueError("invalid y coordinate");

            var color = self.Value.GetPixel((int)x.Value, (int)y.Value);
            return new Collections.Tuple(new Int(color.Red), new Int(color.Green), new Int(color.Blue), new Int(color.Alpha));
        }, [("x", null!), ("y", null!)]));

        field.Set("set_pixel", new NativeFun("set_pixel", 5, field =>
        {
            if (!field[Literals.Self].As<Image>(out var self))
                throw new ValueError("invalid image object");
            if (!field["x"].As<Int>(out var x))
                throw new ValueError("invalid x coordinate");
            if (!field["y"].As<Int>(out var y))
                throw new ValueError("invalid y coordinate");
            if (!field["r"].As<Int>(out var r))
                throw new ValueError("invalid red value");
            if (!field["g"].As<Int>(out var g))
                throw new ValueError("invalid green value");
            if (!field["b"].As<Int>(out var b))
                throw new ValueError("invalid blue value");
            if (!field["a"].As<Int>(out var a))
                throw new ValueError("invalid alpha value");

            self.Value.SetPixel((int)x.Value, (int)y.Value, new SKColor((byte)r.Value, (byte)g.Value, (byte)b.Value, (byte)a.Value));
            return None;
        }, [("x", null!), ("y", null!), ("r", null!), ("g", null!), ("b", null!), ("a", new Int(255))]));
    
        field.Set("resize", new NativeFun("resize", 2, field =>
        {
            if (!field[Literals.Self].As<Image>(out var self) || self.Value == null)
                throw new ValueError("invalid image object");
            if (!field["width"].As<Int>(out var width) || width.Value <= 0)
                throw new ValueError("invalid width");
            if (!field["height"].As<Int>(out var height) || height.Value <= 0)
                throw new ValueError("invalid height");

            var resized = self.Value.Resize(new SKImageInfo((int)width.Value, (int)height.Value), new SKSamplingOptions(SKCubicResampler.Mitchell));
            return new Image { Value = resized };
        }, [("width", null!), ("height", null!)]));
        
        field.Set("rotate", new NativeFun("rotate", 1, field =>
        {
            if (!field[Literals.Self].As<Image>(out var self) || self.Value == null)
                throw new ValueError("invalid image object");
            if (!field["angle"].As<Int>(out var angle))
                throw new ValueError("invalid angle");

            var matrix = SKMatrix.CreateRotationDegrees(angle.Value, self.Value.Width / 2, self.Value.Height / 2);
            var rotated = new SKBitmap(self.Value.Width, self.Value.Height);
            using var canvas = new SKCanvas(rotated);

            canvas.Clear(SKColors.Transparent);
            canvas.SetMatrix(matrix);
            canvas.DrawBitmap(self.Value, 0, 0);

            return new Image { Value = rotated };
        }, [("angle", null!)]));
        
        field.Set("draw_text", new NativeFun("draw_text", 5, field =>
            {
                if (!field[Literals.Self].As<Image>(out var self))
                    throw new ValueError("invalid argument");
                if (!field["text"].As<Str>(out var text))
                    throw new ValueError("invalid text");
                if (!field["x"].As<Int>(out var x) || !field["y"].As<Int>(out var y))
                    throw new ValueError("invalid position");
                if (!field["size"].As<Int>(out var size))
                    throw new ValueError("invalid size");
                if (!field["color"].As<Str>(out var color))
                    throw new ValueError("invalid color");

                using var canvas = new SKCanvas(self.Value);
                using var font = new SKFont(SKTypeface.Default, size.Value);
                using var paint = new SKPaint()
                {
                    Color = SKColor.Parse(color.Value),
                    IsAntialias = true
                };
                canvas.DrawText(text.Value, x.Value, y.Value, font, paint);
                return None;
            }, [("text", null!), ("x", null!), ("y", null!), ("size", null!), ("color", null!)]));

        field.Set("draw_rect", new NativeFun("draw_rect", 5, field =>
            {
                if (!field[Literals.Self].As<Image>(out var self))
                    throw new ValueError("invalid argument");
                if (!field["x"].As<Int>(out var x) || !field["y"].As<Int>(out var y))
                    throw new ValueError("invalid position");
                if (!field["width"].As<Int>(out var width) || !field["height"].As<Int>(out var height))
                    throw new ValueError("invalid size");
                if (!field["color"].As<Str>(out var color))
                    throw new ValueError("invalid color");

                using var canvas = new SKCanvas(self.Value);
                using var paint = new SKPaint
                {
                    Color = SKColor.Parse(color.Value),
                    Style = SKPaintStyle.Fill
                };
                canvas.DrawRect(x.Value, y.Value, width.Value, height.Value, paint);
                return None;
            }, [("x", null!), ("y", null!), ("width", null!), ("height", null!), ("color", null!)]));
        
        field.Set("draw_line", new NativeFun("draw_line", 5, field =>
        {
            if (!field[Literals.Self].As<Image>(out var self) || self.Value == null)
                throw new ValueError("Invalid image");
            if (!field["x1"].As<Int>(out var x1) || !field["y1"].As<Int>(out var y1) ||
                !field["x2"].As<Int>(out var x2) || !field["y2"].As<Int>(out var y2) ||
                !field["thickness"].As<Int>(out var thickness))
                throw new ValueError("Invalid arguments");

            using var canvas = new SKCanvas(self.Value);
            using var paint = new SKPaint { Color = SKColors.Black, StrokeWidth = thickness.Value };
            canvas.DrawLine(x1.Value, y1.Value, x2.Value, y2.Value, paint);
            return None;
        }, [("x1", null!), ("y1", null!), ("x2", null!), ("y2", null!), ("thickness", null!)]));

        field.Set("draw_circle", new NativeFun("draw_circle", 5, field =>
        {
            if (!field[Literals.Self].As<Image>(out var self) || self.Value == null)
                throw new ValueError("Invalid image");
            if (!field["x"].As<Int>(out var x) || !field["y"].As<Int>(out var y) ||
                !field["radius"].As<Int>(out var radius) || !field["thickness"].As<Int>(out var thickness) ||
                !field["fill"].As<Bool>(out var fill))
                throw new ValueError("Invalid arguments");

            using var canvas = new SKCanvas(self.Value);
            using var paint = new SKPaint { Color = SKColors.Red, StrokeWidth = thickness.Value };
            paint.Style = fill.Value ? SKPaintStyle.Fill : SKPaintStyle.Stroke;
            canvas.DrawCircle(x.Value, y.Value, radius.Value, paint);
            return None;
        }, [("x", null!), ("y", null!), ("radius", null!), ("thickness", null!), ("fill", null!)]));

        field.Set("apply_grayscale", new NativeFun("apply_grayscale", 0, field =>
        {
            if (!field[Literals.Self].As<Image>(out var self) || self.Value == null)
                throw new ValueError("invalid image object");

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
            return new Image { Value = grayBitmap };
        }, [])); 

        field.Set("apply_blur", new NativeFun("apply_blur", 1, field =>
        {
            if (!field[Literals.Self].As<Image>(out var self) || self.Value == null)
                throw new ValueError("Invalid image");
            if (!field["radius"].As<Int>(out var radius))
                throw new ValueError("Invalid arguments");

            using var surface = SKSurface.Create(self.Value.Info);
            using var canvas = surface.Canvas;
            using var paint = new SKPaint
            {
                ImageFilter = SKImageFilter.CreateBlur(radius.Value, radius.Value)
            };
            canvas.DrawBitmap(self.Value, 0, 0, paint);
            self.Value = SKBitmap.Decode(surface.Snapshot().Encode());
            return None;
        }, [("radius", null!)]));
    
        field.Set("crop", new NativeFun("crop", 4, field =>
        {
            if (!field[Literals.Self].As<Image>(out var self) || self.Value == null)
                throw new ValueError("invalid image object");
            if (!field["x"].As<Int>(out var x) || !field["y"].As<Int>(out var y))
                throw new ValueError("invalid x, y coordinates");
            if (!field["width"].As<Int>(out var width) || width.Value <= 0)
                throw new ValueError("invalid width");
            if (!field["height"].As<Int>(out var height) || height.Value <= 0)
                throw new ValueError("invalid height");

            var rect = new SKRectI((int)x.Value, (int)y.Value, (int)(x.Value + width.Value), (int)(y.Value + height.Value));
            var copyBitmap = self.Value.Copy();
            var croppedBitmap = self.Value.ExtractSubset(copyBitmap, rect);
            return new Image { Value = copyBitmap };
        }, [("x", null!), ("y", null!), ("width", null!), ("height", null!)]));
        
        field.Set("adjust_opacity", new NativeFun("adjust_opacity", 1, field =>
        {
            if (!field[Literals.Self].As<Image>(out var self) || self.Value == null)
                throw new ValueError("invalid image object");
            if (!field["opacity"].As<Float>(out var opacity))
                throw new ValueError("invalid opacity value");

            var adjustedBitmap = new SKBitmap(self.Value.Width, self.Value.Height);
            using var canvas = new SKCanvas(adjustedBitmap);
            using var paint = new SKPaint
            {
                ColorFilter = SKColorFilter.CreateBlendMode(SKColors.Black.WithAlpha((byte)(255 * (1 - opacity.Value))), SKBlendMode.DstIn)
            };

            canvas.DrawBitmap(self.Value, 0, 0, paint);
            return new Image { Value = adjustedBitmap };
        }, [("opacity", null!)]));

        field.Set("apply_color_shift", new NativeFun("apply_color_shift", 3, field =>
        {
            if (!field[Literals.Self].As<Image>(out var self) || self.Value == null)
                throw new ValueError("invalid image object");
            if (!field["r"].As<Float>(out var r) || !field["g"].As<Float>(out var g) || !field["b"].As<Float>(out var b))
                throw new ValueError("invalid color shift values");

            var colorMatrix = new float[]
            {
                1, 0, 0, 0, (float)r.Value, 
                0, 1, 0, 0, (float)g.Value,
                0, 0, 1, 0, (float)b.Value,
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
            return new Image { Value = shiftedBitmap };
        }, [("r", null!), ("g", null!), ("b", null!)]));

        field.Set("compare", new NativeFun("compare", 1, field =>
        {
            if (!field[Literals.Self].As<Image>(out var self) || self.Value == null)
                throw new ValueError("invalid image object");
            if (!field["other"].As<Image>(out var other) || other.Value == null)
                throw new ValueError("invalid image object");

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
        }, [("other", null!)]));

        field.Set("to_ascii", new NativeFun("to_ascii", 0, field =>
        {
            if (!field[Literals.Self].As<Image>(out var self) || self.Value == null)
                throw new ValueError("invalid image object");

            string ascii = "@%#*+=-:. ";

            var bitmap = self.Value;
            var sb = new StringBuffer();

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var color = bitmap.GetPixel(x, y);
                    int brightness = (color.Red + color.Green + color.Blue) / 3; // 평균 밝기 계산
                    int index = brightness * (ascii.Length - 1) / 255; // 밝기를 문자 인덱스로 변환
                    sb.Append(ascii[index]);
                }
                sb.AppendLine();
            }

            return new Str(sb.ToString());
        }, []));
    }

    public override Obj Init(Collections.Tuple args, Field field) => throw new ClassError("cannot be created. use image.new or image.load");

    public override Obj Clone() => new Image()
    {
        Value = Value.Copy(),
    };

    public override Obj Copy() => this;

    public Obj Static()
    {
        Obj image = new(ClassName);
        image.field.Set("new", new NativeFun("new", 2, field =>
        {
            if (!field["width"].As<Int>(out var width))
                throw new ValueError("invalid width");
            if (!field["height"].As<Int>(out var height))
                throw new ValueError("invalid height");

            var bitmap = new SKBitmap((int)width.Value, (int)height.Value);
            return new Image { Value = bitmap };
        }, [("width", null!), ("height", null!)]));
        image.field.Set("load", new NativeFun("load", 1, field =>
        {
            if (!field["path"].As<Str>(out var path))
                throw new ValueError("invalid file path");
            if (!File.Exists(path.Value))
                throw new FileError("A file that doesn't exist.");

            using var stream = new SKFileStream(path.Value);
            var bitmap = SKBitmap.Decode(stream);
            return new Image(bitmap ?? throw new ValueError("failed to load image"));
        }, [("path", null!)]));
        return image;
    }
}
