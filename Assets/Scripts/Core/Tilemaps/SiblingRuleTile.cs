using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Tilemaps
{
    [CreateAssetMenu(fileName = "Sibling Rule Tile", menuName = "2D/Tiles/Sibling Rule Tile", order = 1)]
    public class SiblingRuleTile : RuleTile
    {
        public enum SiblingGroup
        {
            BankWall,
            Other,
        }
        public SiblingGroup siblingGroup;

        public override bool RuleMatch(int neighbor, TileBase other)
        {
            if (other is RuleOverrideTile)
                other = (other as RuleOverrideTile).m_InstanceTile;

            switch (neighbor)
            {
                case TilingRule.Neighbor.This:
                    {
                        return other is SiblingRuleTile
                            && (other as SiblingRuleTile).siblingGroup == this.siblingGroup;
                    }
                case TilingRule.Neighbor.NotThis:
                    {
                        return !(other is SiblingRuleTile
                            && (other as SiblingRuleTile).siblingGroup == this.siblingGroup);
                    }
            }

            return base.RuleMatch(neighbor, other);
        }
    }
}
