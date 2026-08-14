using System.Runtime.InteropServices;

internal static class Program
{
    [StructLayout(LayoutKind.Sequential)]
    private struct OrbRenderFrame
    {
        public int Width;
        public int Height;
        public int State;
        public float TimeSeconds;
    }

    [DllImport("CodexQuotaViewRendering.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int orb_render_frame(ref OrbRenderFrame frame, byte[] rgbaOut);

    private static int Main()
    {
        const int width = 64;
        const int height = 64;
        var frame = new OrbRenderFrame { Width = width, Height = height, State = 0, TimeSeconds = 0.5f };
        var pixels = new byte[width * height * 4];
        var result = orb_render_frame(ref frame, pixels);
        if (result == 0)
        {
            Console.Error.WriteLine("rendering smoke: orb_render_frame failed");
            return 1;
        }
        var nonTransparent = 0;
        for (var i = 0; i < pixels.Length; i += 4)
        {
            if (pixels[i + 3] > 0)
            {
                nonTransparent++;
            }
        }
        Console.WriteLine($"rendering smoke: ok, opaque_pixels={nonTransparent}");
        return nonTransparent > 0 ? 0 : 1;
    }
}
