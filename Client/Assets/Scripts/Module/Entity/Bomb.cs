using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public MeshRenderer mesh;
    public static int radius = 3;
    public int idx;
    public BombState state;
    private bool highlight = false;

    public void SendIgnite()
    {
        state = BombState.Burning;
        MsgBombState msg = new MsgBombState();
        msg.id = idx;
        msg.state = BombState.Burning;
        NetManager.Send(msg);
    }

    public void Ignite()
    {
        StartCoroutine(DelayIgnite());
    }

    IEnumerator DelayIgnite()
    {
        float highTimer = 0;
        while(state == BombState.Burning)
        {
            highTimer += Time.deltaTime;
            if(highTimer > 0.5f)
            {
                highTimer = 0;
                highlight = !highlight;
                if (highlight) mesh.material.color = Color.white;
                else mesh.material.color = Color.red;
            }
            yield return null;
        }
    }

    public void Explode()
    {
        Destroy(gameObject);
    }
}