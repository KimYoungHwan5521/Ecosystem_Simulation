using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    protected static Dictionary<ResourceEnum.Prefab, Queue<GameObject>> poolDictionary = new();

    Transform poolContainer;
    public IEnumerator Initiate()
    {
        if (poolContainer == null) poolContainer = new GameObject("Pool Container").transform;
        yield return null;
    }

    //                                              � ������, �� ��
    public IEnumerator ClaimPool(Dictionary<ResourceEnum.Prefab, int> dictionary, int numbersOnAFrame = 7)
    {
        if (numbersOnAFrame < 1) numbersOnAFrame = 1;
        int count = 0;
        foreach (var keyValue in dictionary)
        {
            for (int i = 0; i < keyValue.Value; i++)
            {
                ReadyStock(keyValue.Key);
                GameManager.ClaimLoadInfo($"{keyValue.Key} : {count} / {keyValue.Value}");
                count++;
                if (count % numbersOnAFrame == 0)
                {
                    yield return null;
                }
            }
        }
        yield return null;
    }

    protected static void ReadyStock(ResourceEnum.Prefab target)
    {
        // ��ųʸ��� Ű�� ������
        if (poolDictionary.TryGetValue(target, out Queue<GameObject> result))
        {
            poolDictionary[target] = result;
        }
        else
        {
            // ������ ť�� �����ؼ� Ű�� �ֱ�
            Queue<GameObject> queue = new Queue<GameObject>();
            poolDictionary.Add(target, queue);
        }

        GameObject inst = GameObject.Instantiate(ResourceManager.Get(target));
        inst.AddComponent<PoolingInfo>().SetInfo(target, result);
        inst.SetActive(false);
        poolDictionary[target].Enqueue(inst);
    }

    public static GameObject Spawn(ResourceEnum.Prefab target)
    {
        if (poolDictionary.TryGetValue(target, out Queue<GameObject> result))
        {
            if (result.Count == 0)
            {
                ReadyStock(target);
                poolDictionary.TryGetValue(target, out result);
            }
        }
        else
        {
            // ������ ��������
            Queue<GameObject> queue = new();
            poolDictionary.Add(target, queue);
            ReadyStock(target);
            poolDictionary.TryGetValue(target, out result);
        }
        GameObject inst = result.Dequeue();
        inst.SetActive(true);
        return inst;
    }
    public static GameObject Spawn(ResourceEnum.Prefab target, Vector3 pos)
    {
        GameObject inst = Spawn(target);
        inst.transform.position = pos;
        return inst;
    }
    public static GameObject Spawn(ResourceEnum.Prefab target, Vector3 pos, Vector3 euler)
    {
        GameObject inst = Spawn(target, pos);
        inst.transform.eulerAngles = euler;
        return inst;
    }
    public static GameObject Spawn(ResourceEnum.Prefab target, Transform parent)
    {
        GameObject inst = Spawn(target);
        // ��Ȱ��� Ǯ�� �������� Ʈ������ ���� �ٲ���� �� �����Ƿ� ���� �������� ������ �θ� �־��ش�
        GameObject origin = ResourceManager.Get(target);
        inst.transform.SetParent(parent);
        inst.transform.SetLocalPositionAndRotation(origin.transform.position, origin.transform.rotation);
        inst.transform.localScale = origin.transform.localScale;

        return inst;
    }
    public void Destroy(GameObject target)
    {
        if (target.TryGetComponent<PoolingInfo>(out PoolingInfo pool))
        {
            if (PoolManager.poolDictionary.TryGetValue(pool.Origin, out Queue<GameObject> result))
            {
                result.Enqueue(target);
                target.SetActive(false);
            }
            else
            {
                GameObject.Destroy(target);
            }
        }
        else
        {
            GameObject.Destroy(target);
        }
    }
}

public class PoolingInfo : MonoBehaviour
{
    private ResourceEnum.Prefab origin;
    public ResourceEnum.Prefab Origin => origin;
    private Queue<GameObject> originPool;

    public void SetInfo(ResourceEnum.Prefab wantOrigin, Queue<GameObject> wantQueue)
    {
        origin = wantOrigin;
        originPool = wantQueue;
    }
}