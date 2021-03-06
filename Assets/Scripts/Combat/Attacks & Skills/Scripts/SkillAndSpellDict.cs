﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SkillId
{
    BasicAttack,
    BasicTease,
    LowTierHeal,
    LowTierResolve
}

[System.Serializable]
public class SkillDict
{
    [SerializeField] private List<BasicSkill> skills = new List<BasicSkill>();

    public List<BasicSkill> BasicSkills => skills;

    public UserSkill Match(SkillId parId)
    {
        if (!skills.Exists(s => s.Id == parId))
        {
            Debug.LogError("No skills with matching id");
        }
        return new UserSkill(BasicSkills.Find(s => s.Id == parId));
    }

    public List<UserSkill> OwnedSkills(List<Skill> skills) => (from basicSkill in BasicSkills
                                                               from skill in skills
                                                               where Equals(basicSkill.Id, skill.Id)
                                                               select new UserSkill(basicSkill)).ToList();
}