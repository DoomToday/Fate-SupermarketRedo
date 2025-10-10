public static class Constants
{
    // Enum for different phrase types + constant for name
    public const int NAME = 0;
    public enum PhraseType
    {
        Name,
        Buying,
        Satisfied,
        Disappointed,
        NoItem
    }

    // Animation parameters
    public const string ANIM_IS_WALKING = "isWalking";

    // Scene names
    public const int SCENE_MAIN_MENU = 0;
    public const int SCENE_MAIN_GAME = 1;
}