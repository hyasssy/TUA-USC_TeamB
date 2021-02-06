using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class GetAllChildrenExt
{
    ///<summary>
    ///return all active children objs(excluding the argument) from an GameObject.
    ///</summary>
    public static GameObject[] GetAllActiveChildren(this GameObject self)//thisをつけるとGameObject型の拡張になる
    {
        GameObject[] allActiveChildren = self.GetComponentsInChildren<Transform>()//activeselfな子transformを下の階層まで全部取得
        .Where(c => c != self.transform)//except self
        .Select(c => c.gameObject)//型変換
        .ToArray();//配列化
        return allActiveChildren;
    }

    ///<summary>
    ///return all inactive children objs(excluding the argument) from an GameObject.
    ///</summary>
    public static GameObject[] GetAllInactiveChildren(this GameObject self)
    {
        var inactiveChildren = self.GetComponentsInChildren<Transform>(true)//include inactive
        .Where(c => c != self.transform && !c.gameObject.activeSelf);//except self and active objs
        //ここまでだと、inactiveなobjの子が取得できない。
        List<GameObject> list = inactiveChildren.Select(c => c.gameObject).ToList();

        foreach (Transform t in inactiveChildren)
        {
            var children = t.GetComponentsInChildren<Transform>(true)
            .Where(c => c != t.transform)
            .ToList();
            foreach (var child in children)
            {
                list.Add(child.gameObject);
            }
        }
        return list.ToArray();
    }
}
