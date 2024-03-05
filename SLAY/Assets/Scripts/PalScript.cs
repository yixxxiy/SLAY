using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PalType
{
    public string name;
    public string description;
    public int maxHp;
    public int maxHunger;
    public int money;
    public int baseLevel; //初始等级
    public Dictionary<string, int> workSkills;
    public List<string> battleSkills;

    public PalType(string name, string desc, int maxHp, int maxHunger, int money)
    {
        this.name = name;
        this.description = desc;
        this.maxHp = maxHp;
        this.maxHunger = maxHunger;
        this.money = money;

        this.baseLevel = 0;
        workSkills = new Dictionary<string, int>();
        battleSkills = new List<string>();
    }
}

public static class PalTypes
{
    public static Dictionary<string, PalType> palTypes;

    static PalTypes()
    {
        palTypes = new Dictionary<string, PalType>(); ;
        palTypes.Add("普通帕鲁", new PalType("普通帕鲁", "", 100, 100, 10));
        palTypes.Add("搬运帕鲁", new PalType("搬运帕鲁", "", 120, 120, 10));
        palTypes.Add("制作帕鲁", new PalType("制作帕鲁", "", 80, 80, 10));
        palTypes.Add("料理帕鲁", new PalType("料理帕鲁", "", 90, 90, 10));
        palTypes.Add("金币帕鲁", new PalType("金币帕鲁", "", 150, 150, 100));
    }
}

public struct PalBattleSkill
{
    public string name;
    public string description;
    public int damage;

    public PalBattleSkill(string name, string description, int damage)
    {
        this.name = name;
        this.description = description;
        this.damage = damage;
    }
}

public static class PalBattleSkills
{
    public static Dictionary<string, PalBattleSkill> skills;

    public static void Init()
    {
        skills = new Dictionary<string, PalBattleSkill>();
        skills.Add("技能1", new PalBattleSkill("技能1", "", 35));
        skills.Add("技能2", new PalBattleSkill("技能2", "", 50));
    }
}

public class Pal
{
    public string typeName;
    public int health;
    public int hunger;
    public int san;
    public int level;
    public int exp;
    public bool isCaptured; //是否被玩家抓住了

    public const int MAX_SAN = 100;  // 最大心情值
    public const int MAX_EXP = 100;  // 经验条

    public Dictionary<string, int> workSkills = new Dictionary<string, int>();
    public List<string> battleSkills = new List<string>();

    public PalType type
    {
        get
        {
            return PalTypes.palTypes[typeName];
        }
    }

    // 设置工作技能
    public void SetWorkSkill(string skillName, int skillLevel)
    {
        workSkills[skillName] = skillLevel;
    }

    // 移除工作技能
    public void RemoveWorkSkill(string skillName)
    {
        workSkills.Remove(skillName);
    }

    // 添加战斗技能
    public void AddBattleSkill(string skillName)
    {
        if (!battleSkills.Contains(skillName))
        {
            battleSkills.Add(skillName);
        }
    }

    // 移除战斗技能
    public void RemoveBattleSkill(string skillName)
    {
        battleSkills.Remove(skillName);
    }

    // 获得经验值
    public void GainExp(int amount)
    {
        exp += amount;
        if (exp >= MAX_EXP)
        {
            exp = 0;
            LevelUp();
        }
    }

    // 等级增加
    private void LevelUp()
    {
        level++;
        // 执行其他升级相关的操作
    }

    //治疗帕鲁
    public void Heal(int amount)
    {
        health += amount;
        if (health > type.maxHp)
        {
            health = type.maxHp;
        }
    }

    public void Hurt(int amount)
    {
        health -= amount;
        if (health < 0)
        {
            health = 0;
        }
    }
}

public enum PalState
{
    Idle,
    Working,
    Moving,
    Fighting,
    Dead
}

public class PalScript : MonoBehaviour
{
    public Pal pal;
    public PalState state;
    public float speed = 2f;
    public bool moveWhileIdle = true;

    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        state = PalState.Idle;

        pal = new Pal();
        pal.typeName = "普通帕鲁";
        pal.health = pal.type.maxHp;
        pal.hunger = 0;
        pal.san = Pal.MAX_SAN;
        pal.level = 1;
        pal.exp = 0;
        pal.isCaptured = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case PalState.Idle:
                if (moveWhileIdle)
                {
                    targetPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
                    state = PalState.Moving;
                }
                break;
            case PalState.Working:
                // 处理Working状态下的逻辑
                break;
            case PalState.Fighting:
                break;
            case PalState.Moving:
                MoveToTarget(targetPosition);
                break;
            case PalState.Dead:
                break;
            default:
                break;
        }

    }

    private void MoveToTarget(Vector3 target)
    {
        float step = speed * Time.deltaTime; // 移动速度
        transform.position = Vector3.MoveTowards(transform.position, target, step); // 移动至目标位置

        if (Vector3.Distance(transform.position, target) < 0.001f) // 到达目标位置
        {
            state = PalState.Idle;
        }
    }

    //帕鲁工作
    public void Work()
    {

    }

    //帕鲁战斗
    public void Fight()
    {

    }

    public void Hurt(int amount)
    {
        if (state == PalState.Dead)
        {
            return;
        }

        state = PalState.Fighting;
        pal.Hurt(amount);
        Debug.Log(string.Format("Pal {0} get hurt, damage: {1}, health: {2}", name, amount, pal.health));
        if (pal.health == 0 )
        {
            this.Die();
        }
    }

    void Die()
    {
        // 当帕鲁死亡执行的操作
        Debug.Log("啊我死了");
        state= PalState.Dead;
        Destroy(gameObject);
    }
}
