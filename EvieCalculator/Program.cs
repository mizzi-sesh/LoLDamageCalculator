using System.Collections;
using System.ComponentModel;
using System.Configuration.Assemblies;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using ChampDamageCalculator;
using static System.Console;


namespace ChampDamageCalculator{

    public class Evelynn : Champion {
        private Evelynn(List<Ability> abilities, StatLine statLine) : base(abilities, statLine){}
        public static Evelynn Base() {
            StatLine statL = new StatLine
            {
                HP = new(642),
                HP_PerLevel = new(98),
                HP_Per5 = new(8.5f),
                HP_Per5PerLevel = new(0.75f),
                Mana = new(315),
                Mana_PerLevel = new(42),
                Mana_Per5 = new (8.11f),
                Mana_Per5PerLevel = new (0.6f),
                Armor = new(37),
                ArmorPerLevel = new(4.7f),
                MagicResist = new(32),
                MagicResistPerLevel = new(2.05f),
                BaseAttackDamage = new(61),
                AttackPerLevel = new(3),
                AttackRange = new(125),
                MoveSpeed = new (335),
            };

            return new Evelynn(new List<Ability>(){new HateSpike_Dart()}, statL);
        }
    

    }

    public static class Utilities{
        public static T[] PermeateArray<T>(T value, uint size = 5){

           T[] output;

            if (size == 0){
                output = Array.Empty<T>();                
            }
            else{
                output = new T[size];    
            }

            for (int i = 0; i < size; i++){
                output[i] = value;
            }
            return output;
        }
    }
    public static class ComputeDamage{
       public static void Main(){

        var evieInstance2 = Evelynn.Base();

        var evieInstance = Evelynn.Base();
                evieInstance.Level = 1;
                evieInstance.abilityBinds[AbilitySlot.Q].Rank = 1;
            
        Champion eve1 = evieInstance;
        LiveEntity eve2 = evieInstance2;

        evieInstance.abilityBinds[AbilitySlot.Q].Activate(ref eve1, ref eve2, true);

       }    

    }

    public abstract class LiveEntity{
        public StatLine Stats = new();

        public LiveEntity(){

        }

        public List<Effect> Effects = new();

        public void TakeDamage(LiveEntity receivedFrom, float amount, DamageType type){
            
            float preMitDamage = amount;
            float multiplier = 0f;

            //TODO: calculate percentage MPen
            float postPenMR = Stats.MagicResist.Value - receivedFrom.Stats.MagicPentration.Value; 
            //TODO: Calculate lethality and armor pen values;

            switch (type){
                case DamageType.Magical:
                    if (postPenMR >= 0)
                        multiplier = 100f / (100f + postPenMR);
                    else
                        multiplier = 2 - (100f / (100f + postPenMR));
                    break;
                case DamageType.Physical:
                    if (Stats.Armor.Value >= 0)
                        multiplier = 100f / (100f + Stats.Armor.Value);
                    else
                        multiplier = 2 - (100f / (100f + Stats.Armor.Value));
                    break;
                case DamageType.True:
                    multiplier = 1;
                    break;
                default: 
                    multiplier = 0;
                    break;
            }

            WriteLine("HP Before: {0}", Stats.HP.Value);
            Stats.HP.Value -= preMitDamage * multiplier;
            WriteLine("HP After: {0}", Stats.HP.Value);

        }

    }

    public abstract class Champion : LiveEntity {
        private uint _level = 1; 
        public uint Level  {get {return _level;}  set { 
                if (value > _level && value + _level <= MAX_LEVEL) {
                    _level = value;    
                }
            } 
        }
        public const uint MAX_LEVEL = 18;
        private List<Ability> _abilities;

        public Dictionary<AbilitySlot, Ability> abilityBinds = new(5);  
        
        private StatLine _statLine { get { return Stats; } set { Stats = value;} }

        public StatLine StatLine { get { return Stats; } set { Stats = value;}}

        public Champion(List<Ability> abilities, StatLine statLine){

            _abilities = abilities;
            _statLine = statLine;

            foreach (Ability a in abilities){
                if(!abilityBinds.ContainsKey(a.Slot))
                    abilityBinds.Add(a.Slot, a);
            }
        }

    }

    public enum AbilitySlot {
        Passive,
        Q,
        W,
        E,
        R,
        Undefined,
    }

    [Flags]
    public enum CanEffect {
        Self,
        EnemyChampion, 
        AllyChampion, 
        Monsters, 
        Minions
    }

    public enum DamageType { 
        True,
        Physical,
        Magical
    }

    [Flags]
    public enum Trigger {
        State, 
        AutoAttacks,
        Abilities, 
        Time,
    }
    public enum RangeType {
        Melee,
        Ranged
    }

    public abstract class Stat {
      public float Value = 0;   
      public Stat(float value) {Value = value;}
    }

    public enum StatType {
        HP,
        HP_Per5,
        HP_Per5PerLevel,
        HP_PerLevel,
        Mana,
        Mana_Per5,
        Mana_PerLevel,
        Mana_Per5PerLevel,
        HealAndShieldPower,
        Lethality,
        ArmorPenetration,
        MPenFlat, 
        MagicPentration,
        LifeSteal,
        Omnivamp,
        AttackRange,
        Tenacity,
        BaseAttackDamage,
        BonusAttackDamage,
        AttackPerLevel,
        AbilityPower,
        Armor,
        ArmorPerLevel,
        MagicResist,
        MagicResistPerLevel,
        AttackSpeed,
        AbilityHaste,
        CriticalStrikeChance,
        CriticalStrikeDamage,
        MoveSpeed
    }

    public class StatLine {
        public HP HP = new();
        public HP_PerLevel HP_PerLevel = new();
        public HP_Per5 HP_Per5 = new();
        public HP_Per5PerLevel HP_Per5PerLevel = new(); 

        public Mana Mana = new();
        public Mana_Per5 Mana_Per5 = new(); 
        public Mana_PerLevel Mana_PerLevel = new();
        public Mana_Per5PerLevel Mana_Per5PerLevel = new();
        
        public HealAndShieldPower HealAndShieldPower = new();
        public Lethality Lethality = new(); 
        public ArmorPenetration ArmorPenetration= new();
        public MPenFlat MPenFlat = new();
        public MagicPentration MagicPentration = new();

        public LifeSteal LifeSteal = new();
        public Omnivamp Omnivamp = new();

        public Tenacity Tenacity = new();

        public AttackRange AttackRange = new();
        public AttackPerLevel AttackPerLevel = new();
        public BaseAttackDamage BaseAttackDamage = new();

        public AbilityPower AbilityPower = new();

        public Armor Armor = new();
        public ArmorPerLevel ArmorPerLevel = new();
        
        public MagicResist MagicResist = new();
        public MagicResistPerLevel MagicResistPerLevel = new();

        public AttackSpeed AttackSpeed = new();
        public AbilityHaste AbilityHaste = new();

        public CriticalStrikeChance CriticalStrikeChance = new();
        public CriticalStrikeDamage criticalStrikeDamage = new();

        public MoveSpeed MoveSpeed = new();

    }

    public partial class HP(float value=0) : Stat(value){ }; 
    public partial class HP_PerLevel(float value=0) : Stat(value){ }; 
    public partial class HP_Per5(float value=0) : Stat(value){ };
    public partial class HP_Per5PerLevel(float value=0) : Stat(value){ };
    public partial class Mana(float value=0) : Stat(value){ };
    public partial class Mana_PerLevel(float value=0) : Stat(value){ };
    public partial class Mana_Per5(float value=0) : Stat(value){ };
    public partial class Mana_Per5PerLevel(float value=0) : Stat(value){ };
    public partial class HealAndShieldPower(float value=0) : Stat(value){ };
    public partial class Lethality(float value=0) : Stat(value){ };
    public partial class ArmorPenetration(float value=0) : Stat(value){ };
    public partial class MPenFlat(float value=0) : Stat(value){ };
    public partial class MagicPentration(float value=0) : Stat(value){ };
    public partial class LifeSteal(float value=0) : Stat(value){ };
    public partial class Omnivamp(float value=0) : Stat(value){ };
    public partial class AttackRange(float value=0) : Stat(value){ };
    public partial class Tenacity(float value=0) : Stat(value){ };
    public partial class AttackPerLevel (float value=0) : Stat(value){ };
    public partial class BaseAttackDamage(float value=0) : Stat(value){ };
    public partial class BonusAttackDamage(float value=0) : Stat(value){ };
    public partial class AbilityPower(float value=0) : Stat(value){ };
    public partial class Armor(float value=0) : Stat(value){ };
    public partial class ArmorPerLevel(float value=0) : Stat(value){ };
    public partial class MagicResist(float value=0) : Stat(value){ };
    public partial class MagicResistPerLevel(float value=0) : Stat(value){ };
    public partial class AttackSpeed(float value=0) : Stat(value){ };
    public partial class AbilityHaste(float value=0) : Stat(value){ };
    public partial class CriticalStrikeChance(float value=0) : Stat(value){ };
    public partial class CriticalStrikeDamage(float value=0) : Stat(value){ };
    public partial class MoveSpeed(float value=0) : Stat(value){ };


    public abstract class Effect {
       protected CanEffect _validTargets;
       public CanEffect ValidTargets {get {return _validTargets;}}   
       //Heals a live entity.
       public partial class Heal : Effect;
       //Improves static attributes of a live entity.
       public partial class Buff : Effect;
       //Deminishes static attributes of a live entity.
       public partial class Debuff : Effect;
       //Improves the attributes of followup effects.
       public partial class Enhance : Effect;
       //Deminishes the attributes of followup effects.
       public partial class Diminish : Effect;
       public partial class Mark : Effect;
    }

     public partial class Damage : Effect {
        
        public DamageType DealsDamageIn {get { return _dealsDamageIn;}}
        private DamageType _dealsDamageIn;
        public uint[] Amount {get { return _amount; } }
        private uint[] _amount; 
        private StatType? _multipliesWith;
        private float? _multiplicationRatio = 1f;

        public Damage(uint[] amount, DamageType dealsDamageIn, StatType? multipliesWith = null , float? multiplicationRatio = 1f){

            _multiplicationRatio = multiplicationRatio;
            _multipliesWith = multipliesWith;

            _amount = amount;
            _dealsDamageIn = dealsDamageIn;
        }
     }

    public partial class Enhance : Effect {

    }

    public partial class Mark : Effect {
        private bool _hasDuration;
        private float? _duration;
        public float? Duration {get { return _duration; }}
        public Effect Effect;
        private bool _canBeTriggered;
        private Trigger? _validTrigger;
        public Trigger? ValidTrigger {get {return _validTrigger;}}

        public Mark(CanEffect targets, Effect effect, float? duration = null, Trigger? trigger = null){

            _validTargets = targets;
            Effect = effect;
            _duration = duration ?? null; 
            if (_duration == null) _hasDuration = false;
            _validTrigger = trigger ?? null; 
            if (_validTrigger == null) _canBeTriggered = false;
        }

    }


    public abstract class Ability  {
        protected string _name;
        protected AbilitySlot _slot;
        public AbilitySlot Slot {get { return _slot;} }
        protected float[] _baseCooldown;
        protected uint[] _manaCost;
        protected uint[] _baseDamage;  
        protected float[] _damageMultiplier;
        protected StatType? _scalesWith;
        protected Ability? _onPress;
        public bool OnCoolDown = false;
        protected Effect[]? _effects;
        protected readonly uint _maxRank = 5;

        protected uint _rank = 0; 
        public uint Rank {get { return _rank; } 
            set {
                if ( value < _maxRank) {_rank = value;}
                else _rank = _maxRank;
            }
        }

        public abstract void Activate(ref Champion champion, ref LiveEntity target, bool targetHit = true);


        protected Ability(){

            _name = "Undefined";
            _slot = AbilitySlot.Undefined;
            _baseCooldown = [float.MaxValue];
            _manaCost = [uint.MaxValue];
            _baseDamage = [uint.MaxValue];
            _damageMultiplier = [float.MaxValue];
        }
    }

    public class HateSpike_Dart : Ability {
        public string Name { get { return _name; } private set { _name = value; } }
        
        public HateSpike_Dart(){

            _name = "Hate Spike";
            _slot = AbilitySlot.Q; 
            _baseCooldown = Utilities.PermeateArray(4f);
            _baseDamage = [25, 30, 35, 40, 45];
            _manaCost = [40,45,50,55,60];
            _damageMultiplier = Utilities.PermeateArray(0.25f);
            _scalesWith = StatType.AbilityPower;
            HateSpike_Recast.numberOfUses = 3;
            _onPress = new HateSpike_Recast();

            var qMark = new Mark(CanEffect.EnemyChampion | CanEffect.Monsters | CanEffect.Minions, 
                    new Damage([15, 25, 35, 45, 55], 
                        DamageType.Magical, 
                        StatType.AbilityPower, 
                        0.25f),
                    4,
                    Trigger.AutoAttacks | Trigger.Abilities);
            _effects = [qMark, qMark, qMark];
        }

        public override void Activate(ref Champion champion, ref LiveEntity target, bool targetHit = true)
        {
            if (Rank < 1) return;

            if (OnCoolDown) return;

            if (targetHit){

                float preMitigationTotalDamage = 0;

                preMitigationTotalDamage += _baseDamage[Rank];
                preMitigationTotalDamage += _damageMultiplier[Rank] * champion.StatLine.AbilityPower.Value;

                if (_effects != null){
                    target.Effects.AddRange(_effects);
                }

                target.TakeDamage(champion, preMitigationTotalDamage, DamageType.Magical);

                WriteLine("PreMitDamage: {0}.", preMitigationTotalDamage);
            
            }

            champion.abilityBinds[AbilitySlot.Q] = new HateSpike_Recast();
        }
    }

    public class HateSpike_Recast : Ability {
             
        public string Name { get { return _name; } private set { _name = value; } }
        public static uint  numberOfUses = 3;
        public HateSpike_Recast() {

            _name = "Hate Spike";
            _slot = AbilitySlot.Q;
            _baseCooldown = Utilities.PermeateArray(4f);
            _baseDamage = [25, 30, 35, 40, 45]; 
            _scalesWith = StatType.AbilityPower;
            _damageMultiplier = Utilities.PermeateArray(0.25f);
            _manaCost = Utilities.PermeateArray(uint.Parse("0")); 
        }

        public override void Activate(ref Champion champion, ref LiveEntity target, bool targetHit = true)
        {
            if (numberOfUses > 0){
                numberOfUses -= 1; 
                if (numberOfUses == 0){
                     OnCoolDown = true;
                     champion.abilityBinds[AbilitySlot.Q] = new HateSpike_Dart(){OnCoolDown = true};
                     return;
                }
            }
                else 
                    return;

            if (Rank < 1)
                return;

            if (OnCoolDown) return;


            float preMitigationTotalDamage = 0;

            preMitigationTotalDamage += _baseDamage[Rank];
            preMitigationTotalDamage += _damageMultiplier[Rank] * champion.StatLine.AbilityPower.Value;

            target.TakeDamage(champion, preMitigationTotalDamage, DamageType.Magical);
            Console.WriteLine("PreMitDamage: {0}.", preMitigationTotalDamage);
        }
    }
}
