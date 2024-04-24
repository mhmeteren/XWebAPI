using System.ComponentModel;

namespace Entities.Enums
{
    public enum FolderPaths
    {
        [Description("/Users/BackgroundImages/")]
        UsersBackgroundImages,

        [Description("/Users/ProfileImages/")]
        UsersProfileImages,

        [Description("/Tweets/")]
        Tweets
    }
}
