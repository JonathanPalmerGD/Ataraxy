using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModifierManager : Singleton<ModifierManager>
{
	public Modifier GainNewModifier(int level)
	{
		//return Elite.New();

		Modifier m; //= Bolstered.New();

		int n = Random.Range(0, 22);
		//Debug.Log("Index of new modifier: " + n + "\n");
		switch (n)
		{
			case 0:
				m = Bolstered.New();
				break;
			case 1:
				m = Mentor.New();
				break;
			case 2:
				m = Adapting.New();
				break;
			case 3:
				m = Alert.New();
				break;
			case 4:
				m = RapidFire.New();
				break;
			case 5:
				m = Vampiric.New();
				break;
			case 6:
				m = Masochism.New();
				break;
			case 7:
				m = Lucky.New();
				break;
			case 8:
				m = Deadly.New();
				break;
			case 9:
				m = StrongShot.New();
				break;
			case 10:
				m = Regenerating.New();
				break;
			case 11:
				m = WeakShot.New();
				break;
			case 12:
				m = Kamikaze.New();
				break;
			case 13:
				m = Unlucky.New();
				break;
			case 14:
				m = Clumsy.New();
				break;
			case 15:
				m = Oblivious.New();
				break;
			case 16:
				m = Frail.New();
				break;
			case 17:
				m = Rare.New();
				break;
			case 18:
				//Debug.Log("An elite has spawned\n");
				m = Elite.New();
				break;
			case 19:
				m = Durable.New();
				break;
			case 20:
				m = Fragile.New();
				break;
			case 21:
				m = Berserk.New();
				break;


			default:
				m = Bolstered.New();
				break;
		}
		
		m.Init();

		//Debug.Log("Rnd N\t" + n + "\t\t(" + m.Stacks + "x) " + m.ModifierName + "\n");
		return m;
	}
}
