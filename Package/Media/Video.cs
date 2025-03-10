using SkiaSharp;

namespace Un.Package
{
    public class Video : Ref<Obj>, IStatic
    {
        public Video() : base("video", null!) { }

        
        public override void Init()
        {
            
        }

        public override Obj Init(Collections.Tuple args, Field field) => throw new ClassError("cannot be created. use video.new or video.load");

        public override Obj Entry()
        {
            return None;
        }

        public override Obj Exit() 
        {
            return None;
        }

        public Obj Static()
        {
            Obj video = new(ClassName);

            return video;
        }
    }
}
