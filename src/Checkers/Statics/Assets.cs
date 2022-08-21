namespace Checkers.Statics;

/// <summary>
/// Poor mans singleton 😄
/// We could opt for ImageSharp manipulation and just mess with the grayscale to half the amount
/// of images we need, but I find this more easy to work with. Especially with highlighting later on. 
/// </summary>
public static class Assets
{
    private static class Image
    {
        public static CanvasImage Load(string path) =>
            new CanvasImage($"./Images/{path}")
                .MaxWidth(4);
    }

    public static CanvasImage EmptyLightChecker = Image.Load("empty_light_checker.png");
    public static CanvasImage EmptyDarkChecker = Image.Load("empty_dark_checker.png");
    public static CanvasImage FilledLightCheckerWithWhite = Image.Load("filled_light_checker_with_white.png");
    public static CanvasImage FilledDarkCheckerWithWhite = Image.Load("filled_dark_checker_with_white.png");
    public static CanvasImage FilledLightCheckerWithBlack = Image.Load("filled_light_checker_with_black.png");
    public static CanvasImage FilledDarkCheckerWithBlack = Image.Load("filled_dark_checker_with_black.png");
}