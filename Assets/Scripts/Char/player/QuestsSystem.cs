﻿using System.Collections.Generic;

public enum Quests
{
    Bandit,
    ElfsHunt
}

public static class QuestsSystem
{
    public static List<BasicQuest> List { get; private set; } = new List<BasicQuest>();

    public static bool HasQuest(Quests parQuest) => List.Exists(q => q.Type == parQuest);

    public static BasicQuest GetQuest(Quests parQuest) => List.Find(q => q.Type == parQuest);

    public static void AddQuest(Quests which)
    {
        switch (which)
        {
            case Quests.Bandit:
                if (!HasQuest(Quests.Bandit))
                {
                    List.Add(new BanditQuest());
                }
                break;

            case Quests.ElfsHunt:
                if (!HasQuest(Quests.ElfsHunt))
                {
                    List.Add(new ElfQuest());
                }
                break;
        }
    }

    public static QuestSave Save() => new QuestSave(List);

    public static void Load(QuestSave toLoad) => List = new List<BasicQuest>(toLoad.BasicQuests);
}

[System.Serializable]
public struct QuestSave
{
    [UnityEngine.SerializeField]
    private List<BasicQuest> basicQuests;

    public QuestSave(List<BasicQuest> parQuests)
    {
        basicQuests = parQuests;
    }

    public List<BasicQuest> BasicQuests => basicQuests;
}

public static class QuestDesc
{
    public static string GetDesc(Quests which)
    {
        switch (which)
        {
            case Quests.Bandit:
                return "The bandit gang up north are getting bolder every day, and it's affecting our trade. We would in your debt if you could teach them a lesson.";

            case Quests.ElfsHunt:
                return "The elfs down south,";

            default: return string.Empty;
        }
    }

    public static string QuestReturnTo(Quests quests)
    {
        switch (quests)
        {
            case Quests.Bandit: return "Townhall";
            case Quests.ElfsHunt: return "Townhall";
            default: return string.Empty;
        }
    }
}