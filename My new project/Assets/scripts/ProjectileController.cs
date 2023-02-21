using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public List<GameObject> projectilePrefabs;
    public List<Vector3> projectilePrefabsInstantiateRotation;
    public List<ProjectileObject> projectiles;
    public Transform respawnPt;
    public float gameElapseTime;
    public float projectileGenerateFreq;

    // TODO: 需要function控制產生一定時間之後的projectile要被destroy
    // 或是該projectile很靠近destroy point之後就要被destroy

    public void OnCollideProjectile(Transform trans)
    {
        ProjectileObject po = trans.GetComponent<ProjectileObject>();
        po.breakConstraint();
        po.releaseSpeedControl();
        destroyProjectile(po, 1);
    }

    public void destroyProjectile(ProjectileObject projectile, float delayTime=1f)
    {
        projectiles.Remove(projectile);
        projectile.delayDestroy(delayTime);
    }

    public void generateProjectile(int ind)
    {
        GameObject instantGO = Instantiate(projectilePrefabs[ind], respawnPt.position, Quaternion.Euler(projectilePrefabsInstantiateRotation[ind]));
        ProjectileObject instantPO = instantGO.GetComponent<ProjectileObject>();
        projectiles.Add(instantPO);
        destroyProjectile(instantPO, 3);
    }

    /// <summary>
    /// 一個遊戲流程 
    /// 不斷產生(隨機)projectile往avatar移動, 
    /// 直到結束遊戲 
    /// </summary>
    /// <returns></returns>
    IEnumerator runGameProcess()
    {
        float gameStartTime = Time.time;
        while(Time.time - gameStartTime < gameElapseTime)
        {
            int tmpRandomInd = Random.Range(0, projectilePrefabs.Count);
            generateProjectile(tmpRandomInd);
            yield return new WaitForSeconds(projectileGenerateFreq);
        }
        yield return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(runGameProcess());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
