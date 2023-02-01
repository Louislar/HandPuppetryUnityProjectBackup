using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public List<GameObject> projectilePrefabs;
    public List<ProjectileObject> projectiles;
    public Transform respawnPt;
    public float gameElapseTime;
    public float projectileGenerateFreq;

    // TODO: �ݭnfunction����ͤ@�w�ɶ����᪺projectile�n�Qdestroy
    // �άO��projectile�ܾa��destroy point����N�n�Qdestroy

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
        GameObject instantGO = Instantiate(projectilePrefabs[ind], respawnPt.position, Quaternion.identity);
        ProjectileObject instantPO = instantGO.GetComponent<ProjectileObject>();
        projectiles.Add(instantPO);
        destroyProjectile(instantPO, 3);
    }

    /// <summary>
    /// �@�ӹC���y�{ 
    /// ���_����(�H��)projectile��avatar����, 
    /// ���쵲���C�� 
    /// </summary>
    /// <returns></returns>
    IEnumerator runGameProcess()
    {
        float gameStartTime = Time.time;
        while(Time.time - gameStartTime < gameElapseTime)
        {
            generateProjectile(0);
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
