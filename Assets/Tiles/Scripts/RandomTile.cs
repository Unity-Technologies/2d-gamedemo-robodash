using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu]
public class RandomTile : TileBase {
    public SpriteSlotItem[] sprites;
    public float perlinInputScale = 1f;
    public float perlinOutputScale = 1f;
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        if (sprites.Length == 0) return;

        
        tileData.colliderType = Tile.ColliderType.Grid;

        Random.InitState(position.GetHashCode());
        int total = sprites.Sum(x => x.probability);

        int[] indices = new int[total];
        int spriteIndex = 0;
        int indiceIndex = 0;
        foreach (var s in sprites) {
            //indiceIndex
            for (int index = 0; index < s.probability; index++)
                indices[indiceIndex++] = spriteIndex;
            //spriteIndex += s.probability;
            spriteIndex++;
        }
        int random = Mathf.FloatToHalf(Random.value * total);
        int finalIndex = indices[Mathf.Clamp(random % total, 0, total - 1)];
        tileData.sprite = sprites[Mathf.Clamp(finalIndex, 0, sprites.Length - 1)].sprite;

    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
        foreach (var p in new BoundsInt(-1, -1, 0, 3, 3, 1).allPositionsWithin) {
            tilemap.RefreshTile(position + p);
        }
    }

    
    [System.Serializable]
    public class SpriteSlotItem {
        public Sprite sprite;
        public int probability = 1;
    }
}
