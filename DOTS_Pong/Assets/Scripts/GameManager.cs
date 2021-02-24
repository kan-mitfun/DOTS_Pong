using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager main;

	public GameObject ballPrefab;

	public float xBound = 3f;
	public float yBound = 3f;
	public float ballSpeed = 3f;
	public float respawnDelay = 2f;
	public int[] playerScores;

	public Text mainText;
	public Text[] playerTexts;

    [Range(0, 5)]
    public int ballAmountOnStart = 0;

	Entity ballEntityPrefab;
	EntityManager manager;

    //WaitForSeconds：特定常用秒數放在全域變數，並在Awake初始化，以減少每次new的耗損。
    WaitForSeconds oneSecond;
	WaitForSeconds delay;

	private void Awake()
	{
		if (main != null && main != this)
		{
			Destroy(gameObject);
			return;
		}

		main = this;
		playerScores = new int[2];

        //取得每次將 GameObject 轉換成 Entity 的 World，其下的 EntityManager
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        //以取得上面同一個 World 的設定。null的參數類型是BlobAssetStore，預期在新版本的DOTS package上可以放入新的物理/動畫系統資源。
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        //用以將GameObjectPrefab轉換為EntityPrefab
		ballEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(ballPrefab, settings);

		oneSecond = new WaitForSeconds(1f);
		delay = new WaitForSeconds(respawnDelay);

        StartCoroutine(CountdownAndSpawnBall(ballAmountOnStart * short.MaxValue));
    }

	public void PlayerScored(int playerID)
	{
		playerScores[playerID]++;
		for (int i = 0; i < playerScores.Length && i < playerTexts.Length; i++)
			playerTexts[i].text = playerScores[i].ToString();

		StartCoroutine(CountdownAndSpawnBall());
	}

	IEnumerator CountdownAndSpawnBall(int ballAmount=1)
	{
		mainText.text = "Get Ready";
		yield return delay;

		mainText.text = "3";
		yield return oneSecond;

		mainText.text = "2";
		yield return oneSecond;

		mainText.text = "1";
		yield return oneSecond;

		mainText.text = "";

        ballAmount = Mathf.Max(1, ballAmount);

        while (ballAmount-- > 0)
        {
            if (ballAmount % 1000 == 0)
            {
                yield return null;
            }
            SpawnBall();
        }
	}

	void SpawnBall()
    {
        //將球實體化並加入遊戲中(World)
        Entity ball = manager.Instantiate(ballEntityPrefab);

        //將球加入物理速度
        Vector3 dir = new Vector3(UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1, UnityEngine.Random.Range(-.5f, .5f), 0f).normalized;
		Vector3 speed = dir * ballSpeed;

		PhysicsVelocity velocity = new PhysicsVelocity()
		{
			Linear = speed,
			Angular = float3.zero
		};

		manager.AddComponentData(ball, velocity);
	}
}

