// The game's sprites were exported as "<texture> 0". Loading by texture name alone gets a
// generated full-texture sprite with the wrong size and pivot.
using UnityEngine;

public static class PortSprites
{
    public static Sprite Load(string path)
    {
        Sprite s = Resources.Load<Sprite>(path + " 0");
        return s != null ? s : Resources.Load<Sprite>(path);
    }
}
