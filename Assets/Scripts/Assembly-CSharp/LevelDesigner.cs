using UnityEngine;

public class LevelDesigner : MonoBehaviour
{
	public class WorldData
	{
		private MathUtils.Range velocity;

		private MathUtils.RangeInt difficulty;

		public string Name { get; private set; }

		public int Length { get; private set; }

		public float LengthAsFloat { get; private set; }

		public bool IsEndless { get; private set; }

		public bool IsNotEndless { get; private set; }

		public float BeatLength { get; private set; }

		public int RowOfMaxVelocity { get; private set; }

		public int DifficultyLength { get; private set; }

		public bool FixedDifficulty { get; private set; }

		public bool VariableDifficulty { get; private set; }

		public bool RandomDifficulty { get; private set; }

		public int RowOfMaxDifficulty { get; private set; }

		public float RowOfMaxDifficultyAsFloat { get; private set; }

		public float VelocityMin
		{
			get
			{
				return velocity.Min;
			}
		}

		public float VelocityMax
		{
			get
			{
				return velocity.Max;
			}
		}

		public float Acceleration { get; private set; }

		public bool VelocityIsMaxed { get; private set; }

		public bool VelocityIsStatic { get; private set; }

		public bool VelocityIsNotMaxed
		{
			get
			{
				return !VelocityIsMaxed;
			}
		}

		public bool VelocityIsDynamic
		{
			get
			{
				return !VelocityIsStatic;
			}
		}

		public int DifficultyMin
		{
			get
			{
				return difficulty.Min;
			}
		}

		public int DifficultyMax
		{
			get
			{
				return difficulty.Max;
			}
		}

		public bool DifficultyIsMaxed { get; private set; }

		public bool DifficultyIsNotMaxed
		{
			get
			{
				return !DifficultyIsMaxed;
			}
		}

		public WorldData(string name)
		{
			ConfigureWorldData(name, null, 1f, 1f, 1f, 6f, null, null, null);
		}

		public WorldData(string name, int? goalLength, float startingVelocityPercent, float maxVelocityPercent, float worldPercentForMaxVelocity, float beatLength)
		{
			ConfigureWorldData(name, goalLength, startingVelocityPercent, maxVelocityPercent, worldPercentForMaxVelocity, beatLength, null, null, null);
		}

		public WorldData(string name, int? goalLength, float startingVelocityPercent, float maxVelocityPercent, float worldPercentForMaxVelocity, float beatLength, int difficultyMin, int difficultyMax)
		{
			ConfigureWorldData(name, goalLength, startingVelocityPercent, maxVelocityPercent, worldPercentForMaxVelocity, beatLength, difficultyMin, difficultyMax, null);
		}

		public WorldData(string name, int? goalLength, float startingVelocityPercent, float maxVelocityPercent, float worldPercentForMaxVelocity, float beatLength, int difficultyMin, int difficultyMax, float worldPercentForMaxDifficulty)
		{
			ConfigureWorldData(name, goalLength, startingVelocityPercent, maxVelocityPercent, worldPercentForMaxVelocity, beatLength, difficultyMin, difficultyMax, worldPercentForMaxDifficulty);
		}

		private void ConfigureWorldData(string name, int? goalLength, float startingVelocityPercent, float maxVelocityPercent, float worldPercentForMaxVelocity, float beatLength, int? difficultyMin, int? difficultyMax, float? worldPercentForMaxDifficulty)
		{
			Name = name;
			IsNotEndless = goalLength.HasValue;
			IsEndless = !IsNotEndless;
			Length = ((!IsNotEndless) ? (-1) : goalLength.Value);
			LengthAsFloat = Length;
			BeatLength = beatLength;
			VelocityIsStatic = startingVelocityPercent == maxVelocityPercent;
			float min = GlobalVelocityRange.FromPercent(startingVelocityPercent);
			float max = GlobalVelocityRange.FromPercent(maxVelocityPercent);
			velocity = new MathUtils.Range(min, max);
			Acceleration = velocity.Average;
			RowOfMaxVelocity = MathUtils.Floored(LengthAsFloat * worldPercentForMaxVelocity);
			if (difficultyMin.HasValue && difficultyMax.HasValue)
			{
				difficulty = new MathUtils.RangeInt(difficultyMin.Value, difficultyMax.Value);
				if (worldPercentForMaxDifficulty.HasValue)
				{
					RowOfMaxDifficultyAsFloat = LengthAsFloat * worldPercentForMaxDifficulty.Value;
					RowOfMaxDifficulty = MathUtils.Floored(RowOfMaxDifficultyAsFloat);
					VariableDifficulty = true;
					RandomDifficulty = false;
				}
				else
				{
					RandomDifficulty = true;
					VariableDifficulty = false;
				}
				FixedDifficulty = false;
			}
			else
			{
				FixedDifficulty = true;
				VariableDifficulty = false;
				RandomDifficulty = false;
			}
			Reset();
		}

		public void Reset()
		{
			DifficultyIsMaxed = FixedDifficulty;
			VelocityIsMaxed = false;
		}

		public int GetDifficulty(int rowNum)
		{
			if (FixedDifficulty)
			{
				return GameManager.WorldIndex;
			}
			if (RandomDifficulty)
			{
				return difficulty.Random;
			}
			if (DifficultyIsMaxed)
			{
				return difficulty.Max;
			}
			if (rowNum >= RowOfMaxDifficulty)
			{
				DifficultyIsMaxed = true;
				return difficulty.Max;
			}
			float num = rowNum;
			float percent = num / RowOfMaxDifficultyAsFloat;
			return MathUtils.Floored(difficulty.FromPercent(percent));
		}

		public float GetVelocity(int rowNum)
		{
			if (VelocityIsMaxed || VelocityIsStatic)
			{
				return velocity.Max;
			}
			bool wasBelow;
			bool wasAbove;
			float percent = MathUtils.ToPercentInt01(rowNum, 0, RowOfMaxVelocity, out wasBelow, out wasAbove);
			if (wasAbove)
			{
				VelocityIsMaxed = true;
			}
			return velocity.FromPercent(percent);
		}

		public float TimeOf(int rowNum)
		{
			if (VelocityIsStatic)
			{
				return (float)rowNum / velocity.Max;
			}
			float num = GetVelocity(rowNum);
			float num2 = (MathUtils.Squared(num) - MathUtils.Squared(velocity.Min)) / (2f * (float)rowNum);
			return (num - velocity.Min) / num2;
		}

		public int RowOf(int secondsPlayed)
		{
			int num;
			if (VelocityIsStatic)
			{
				num = MathUtils.Floored(velocity.Max * (float)secondsPlayed);
			}
			else
			{
				float num2 = TimeOf(Length);
				float max = velocity.Max;
				float min = velocity.Min;
				float num3 = (max - min) / num2;
				float num4 = secondsPlayed;
				float value = min * num4 + 0.5f * num3 * MathUtils.Squared(num4);
				num = MathUtils.Floored(value);
			}
			return num + 3;
		}
	}

	public class Period
	{
		private ArrayUtils.WeightedListRandom<SegmentType> choices;

		private readonly SegmentType? singleChoice;

		public readonly int Time;

		public Period(int periodTime, params SegmentType[] periodChoices)
		{
			Time = periodTime;
			if (periodChoices.Length == 1)
			{
				singleChoice = periodChoices[0];
			}
			else
			{
				choices = new ArrayUtils.WeightedListRandom<SegmentType>(periodChoices);
			}
		}

		public void ClearPrevious()
		{
			if (!singleChoice.HasValue)
			{
				choices.ClearPrevious();
			}
		}

		public SegmentType ChooseSegmentType()
		{
			if (singleChoice.HasValue)
			{
				return singleChoice.Value;
			}
			return choices.GetRandomNext();
		}
	}

	public class WorldRhythm
	{
		private Period[] periods;

		private int currentPeriod;

		private int lastPeriod;

		private bool inLastPeriod;

		public WorldRhythm(params Period[] rhythmPeriods)
		{
			periods = rhythmPeriods;
			lastPeriod = periods.Length - 1;
			Reset();
		}

		public void Reset()
		{
			currentPeriod = 0;
			inLastPeriod = currentPeriod == lastPeriod;
			for (int i = 0; i < periods.Length; i++)
			{
				periods[i].ClearPrevious();
			}
		}

		public SegmentType ChooseSegmentType(float secondsIntoLevel, out bool nextPeriod)
		{
			nextPeriod = false;
			if (!inLastPeriod)
			{
				while ((float)periods[currentPeriod].Time <= secondsIntoLevel)
				{
					currentPeriod++;
					nextPeriod = true;
					if (currentPeriod == lastPeriod)
					{
						inLastPeriod = true;
						break;
					}
				}
			}
			return periods[currentPeriod].ChooseSegmentType();
		}
	}

	public struct Theme
	{
		private const bool defaultGenerateTrees = false;

		private const bool defaultGenerateTowers = false;

		private const bool defaultGenerateAltGround = false;

		private const bool defaultGeneratePyramids = false;

		private const bool defaultGeneratePalmTrees = false;

		private const bool defaultGenerateHammers = true;

		private const bool defaultGenerateSlashers = false;

		private const bool defaultGenerateHammersAndSlashers = false;

		public readonly int WorldThemeNumber;

		public readonly int WorldThemeIndex;

		public readonly int Distance;

		public readonly float TransitionTime;

		public readonly bool GenerateTrees;

		public readonly bool GenerateTowers;

		public readonly bool GenerateAltGround;

		public readonly bool GeneratePyramids;

		public readonly bool GeneratePalmTrees;

		public readonly bool GenerateHammers;

		public readonly bool GenerateSlashers;

		public readonly bool GenerateHammersAndSlashers;

		public Theme(int themeTime, int worldThemeNumber, float transitionTime)
		{
			Distance = themeTime;
			WorldThemeNumber = worldThemeNumber;
			WorldThemeIndex = worldThemeNumber - 1;
			TransitionTime = transitionTime;
			GenerateTrees = false;
			GenerateTowers = false;
			GenerateAltGround = false;
			GeneratePyramids = false;
			GeneratePalmTrees = false;
			GenerateHammers = true;
			GenerateSlashers = false;
			GenerateHammersAndSlashers = false;
		}

		public Theme(int themeTime, int worldThemeNumber, float transitionTime, bool useTrees)
		{
			Distance = themeTime;
			WorldThemeNumber = worldThemeNumber;
			WorldThemeIndex = worldThemeNumber - 1;
			TransitionTime = transitionTime;
			GenerateTrees = useTrees;
			GenerateTowers = false;
			GenerateAltGround = false;
			GeneratePyramids = false;
			GeneratePalmTrees = false;
			GenerateHammers = true;
			GenerateSlashers = false;
			GenerateHammersAndSlashers = false;
		}

		public Theme(int themeTime, int worldThemeNumber, float transitionTime, bool useTrees, bool useTowers)
		{
			Distance = themeTime;
			WorldThemeNumber = worldThemeNumber;
			WorldThemeIndex = worldThemeNumber - 1;
			TransitionTime = transitionTime;
			GenerateTrees = useTrees;
			GenerateTowers = useTowers;
			GenerateAltGround = false;
			GeneratePyramids = false;
			GeneratePalmTrees = false;
			GenerateHammers = true;
			GenerateSlashers = false;
			GenerateHammersAndSlashers = false;
		}

		public Theme(int themeTime, int worldThemeNumber, float transitionTime, bool useTrees, bool useTowers, bool useAltGround)
		{
			Distance = themeTime;
			WorldThemeNumber = worldThemeNumber;
			WorldThemeIndex = worldThemeNumber - 1;
			TransitionTime = transitionTime;
			GenerateTrees = useTrees;
			GenerateTowers = useTowers;
			GenerateAltGround = useAltGround;
			GeneratePyramids = false;
			GeneratePalmTrees = false;
			GenerateHammers = true;
			GenerateSlashers = false;
			GenerateHammersAndSlashers = false;
		}

		public Theme(int themeTime, int worldThemeNumber, float transitionTime, bool useTrees, bool useTowers, bool useAltGround, bool usePyramids, bool usePalmTrees)
		{
			Distance = themeTime;
			WorldThemeNumber = worldThemeNumber;
			WorldThemeIndex = worldThemeNumber - 1;
			TransitionTime = transitionTime;
			GenerateTrees = useTrees;
			GenerateTowers = useTowers;
			GenerateAltGround = useAltGround;
			GeneratePyramids = usePyramids;
			GeneratePalmTrees = usePalmTrees;
			GenerateHammers = true;
			GenerateSlashers = false;
			GenerateHammersAndSlashers = false;
		}

		public Theme(int themeTime, int worldThemeNumber, float transitionTime, bool useTrees, bool useTowers, bool useAltGround, bool usePyramids, bool usePalmTrees, bool useSlashers)
		{
			Distance = themeTime;
			WorldThemeNumber = worldThemeNumber;
			WorldThemeIndex = worldThemeNumber - 1;
			TransitionTime = transitionTime;
			GenerateTrees = useTrees;
			GenerateTowers = useTowers;
			GenerateAltGround = useAltGround;
			GeneratePyramids = usePyramids;
			GeneratePalmTrees = usePalmTrees;
			GenerateHammers = !useSlashers;
			GenerateSlashers = useSlashers;
			GenerateHammersAndSlashers = false;
		}
	}

	public class WorldTheme
	{
		private Theme[] themes;

		private int themeIndexCurrent;

		private int themeIndexGenerating;

		private int themeIndexFinal;

		private bool themeIndexCurrentIsFinal;

		private bool themeIndexGeneratingIsFinal;

		public int CurrentWorldThemeNumber
		{
			get
			{
				return themes[themeIndexCurrent].WorldThemeNumber;
			}
		}

		public int CurrentWorldThemeIndex
		{
			get
			{
				return themes[themeIndexCurrent].WorldThemeIndex;
			}
		}

		public int GeneratingWorldThemeNumber
		{
			get
			{
				return themes[themeIndexGenerating].WorldThemeNumber;
			}
		}

		public int GeneratingWorldThemeIndex
		{
			get
			{
				return themes[themeIndexGenerating].WorldThemeIndex;
			}
		}

		public float CurrentWorldTransition
		{
			get
			{
				return themes[themeIndexCurrent].TransitionTime;
			}
		}

		public bool GenerateTrees
		{
			get
			{
				return themes[themeIndexGenerating].GenerateTrees;
			}
		}

		public bool GenerateTowers
		{
			get
			{
				return themes[themeIndexGenerating].GenerateTowers;
			}
		}

		public bool GeneratePyramids
		{
			get
			{
				return themes[themeIndexGenerating].GeneratePyramids;
			}
		}

		public bool GeneratePalmTrees
		{
			get
			{
				return themes[themeIndexGenerating].GeneratePalmTrees;
			}
		}

		public bool GenerateAltGround
		{
			get
			{
				return themes[themeIndexGenerating].GenerateAltGround;
			}
		}

		public bool GenerateHammers
		{
			get
			{
				return themes[themeIndexGenerating].GenerateHammers;
			}
		}

		public bool GenerateSlashers
		{
			get
			{
				return themes[themeIndexGenerating].GenerateSlashers;
			}
		}

		public bool GenerateHammersAndSlashers
		{
			get
			{
				return themes[themeIndexGenerating].GenerateHammersAndSlashers;
			}
		}

		public WorldTheme(params Theme[] worldThemes)
		{
			themes = worldThemes;
			themeIndexFinal = themes.Length - 1;
			Reset();
		}

		public void Reset()
		{
			themeIndexCurrent = 0;
			themeIndexGenerating = 0;
			themeIndexCurrentIsFinal = themeIndexCurrent == themeIndexFinal;
			themeIndexGeneratingIsFinal = themeIndexGenerating == themeIndexFinal;
		}

		public bool ShouldChangeTheme(int rowsIntoLevel)
		{
			bool result = false;
			if (!themeIndexCurrentIsFinal)
			{
				while (!themeIndexCurrentIsFinal && themes[themeIndexCurrent + 1].Distance <= rowsIntoLevel)
				{
					themeIndexCurrent++;
					result = true;
					if (themeIndexCurrent == themeIndexFinal)
					{
						themeIndexCurrentIsFinal = true;
						break;
					}
				}
			}
			return result;
		}

		public void TryChangeGeneratingTheme(int rowsIntoLevel)
		{
			if (themeIndexGeneratingIsFinal)
			{
				return;
			}
			while (!themeIndexGeneratingIsFinal && themes[themeIndexGenerating + 1].Distance <= rowsIntoLevel)
			{
				themeIndexGenerating++;
				if (themeIndexGenerating == themeIndexFinal)
				{
					themeIndexGeneratingIsFinal = true;
					break;
				}
			}
		}
	}

	private const bool debugsEnabled = false;

	private const bool debugPeriods = false;

	private const bool debugTimesUsed = false;

	private const bool debugChains = false;

	private const bool debugAvailability = false;

	private const bool debugDifficulty = false;

	private const bool debugPausing = false;

	private const bool debugFrequencies = false;

	public const int MaxDifficulty = 5;

	public static MathUtils.Range GlobalVelocityRange = new MathUtils.Range(3.75f, 8f);

	private static WorldData[] worldData = new WorldData[9]
	{
		new WorldData("1", 500, 0.525f, 0.525f, 1f, 6f),
		new WorldData("2", 650, 0.625f, 0.625f, 1f, 7f),
		new WorldData("3", 805, 0.725f, 0.8f, 1f, 5f),
		new WorldData("4", 970, 0.8f, 1f, 1f, 6f),
		new WorldData("5", 1170, 0.85f, 1f, 0.5f, 6f),
		new WorldData("Reverof", 1300, 1f, 1f, 1f, 6f, 3, 5, 0.2f),
		new WorldData("Sikarra"),
		new WorldData("Mugelbbub"),
		new WorldData("Xitram")
	};

	private static WorldRhythm[] worldRhythms = new WorldRhythm[5]
	{
		new WorldRhythm(new Period(100, SegmentType.Level)),
		new WorldRhythm(new Period(100, SegmentType.Level)),
		new WorldRhythm(new Period(100, SegmentType.Level)),
		new WorldRhythm(new Period(100, SegmentType.Level)),
		new WorldRhythm(new Period(100, SegmentType.Level))
	};

	private static WorldTheme[] worldThemes = new WorldTheme[5]
	{
		new WorldTheme(new Theme(0, 1, 1f), new Theme(105, 2, 1f), new Theme(207, 3, 1f, true), new Theme(312, 9, 1f, false, false, false, false, false, false), new Theme(415, 1, 1f)),
		new WorldTheme(new Theme(0, 2, 1f), new Theme(123, 5, 1f, false, true, true), new Theme(306, 4, 1f), new Theme(551, 2, 1f)),
		new WorldTheme(new Theme(0, 3, 1f, true), new Theme(153, 9, 1f, false, false, false, false, false, true), new Theme(275, 9, 1f, false, false, false, false, false, false), new Theme(420, 2, 1f), new Theme(554, 3, 1f, true), new Theme(691, 5, 1f, false, true, true)),
		new WorldTheme(new Theme(0, 4, 1f), new Theme(236, 7, 1f), new Theme(475, 2, 1f), new Theme(598, 9, 1f), new Theme(721, 7, 1f), new Theme(850, 4, 1f)),
		new WorldTheme(new Theme(0, 5, 1f, false, true, true))
	};

	private static int maxDifficultyForPlayer;

	private static int segmentsUntilMaxDifficultyForPlayer;

	private static float difficultyExtender;

	private static int chainedSegmentsLeft;

	private static int? lastChosenTypeNum;

	public static WorldData CurrentWorldData
	{
		get
		{
			return worldData[GameManager.WorldIndex];
		}
	}

	public static int WorldLength
	{
		get
		{
			return CurrentWorldData.Length;
		}
	}

	public static float WorldLengthAsFloat
	{
		get
		{
			return CurrentWorldData.LengthAsFloat;
		}
	}

	public static bool WorldIsEndless
	{
		get
		{
			return CurrentWorldData.IsEndless;
		}
	}

	public static bool WorldIsNotEndless
	{
		get
		{
			return CurrentWorldData.IsNotEndless;
		}
	}

	public static string WorldName
	{
		get
		{
			return CurrentWorldData.Name;
		}
	}

	public static string NextWorldName
	{
		get
		{
			return worldData[GameManager.GetNextWorldIndex()].Name;
		}
	}

	public static string MaxDifficultyAsChar
	{
		get
		{
			return GameManager.IntToChar(5);
		}
	}

	public static float VelocityMin
	{
		get
		{
			return CurrentWorldData.VelocityMin;
		}
	}

	public static float VelocityMax
	{
		get
		{
			return CurrentWorldData.VelocityMax;
		}
	}

	public static bool VelocityIsMaxed
	{
		get
		{
			return CurrentWorldData.VelocityIsMaxed;
		}
	}

	public static bool VelocityIsNotMaxed
	{
		get
		{
			return CurrentWorldData.VelocityIsNotMaxed;
		}
	}

	public static float Acceleration
	{
		get
		{
			return CurrentWorldData.Acceleration;
		}
	}

	public static WorldTheme CurrentWorldTheme
	{
		get
		{
			return worldThemes[GameManager.WorldIndex];
		}
	}

	public static int CurrentWorldThemeNumber
	{
		get
		{
			return CurrentWorldTheme.CurrentWorldThemeNumber;
		}
	}

	public static int CurrentWorldThemeIndex
	{
		get
		{
			return CurrentWorldTheme.CurrentWorldThemeIndex;
		}
	}

	public static float CurrentWorldThemeTransition
	{
		get
		{
			return CurrentWorldTheme.CurrentWorldTransition;
		}
	}

	public static float BeatLength
	{
		get
		{
			return CurrentWorldData.BeatLength;
		}
	}

	public static bool GenerateTrees
	{
		get
		{
			return CurrentWorldTheme.GenerateTrees;
		}
	}

	public static bool GenerateTowers
	{
		get
		{
			return CurrentWorldTheme.GenerateTowers;
		}
	}

	public static bool GenerateAltGround
	{
		get
		{
			return CurrentWorldTheme.GenerateAltGround;
		}
	}

	public static bool GeneratePyramids
	{
		get
		{
			return CurrentWorldTheme.GeneratePyramids;
		}
	}

	public static bool GeneratePalmTrees
	{
		get
		{
			return CurrentWorldTheme.GeneratePalmTrees;
		}
	}

	public static bool GenerateHammers
	{
		get
		{
			return CurrentWorldTheme.GenerateHammers;
		}
	}

	public static bool GenerateSlashers
	{
		get
		{
			return CurrentWorldTheme.GenerateSlashers;
		}
	}

	public static bool GenerateHammersAndSlashers
	{
		get
		{
			return CurrentWorldTheme.GenerateHammersAndSlashers;
		}
	}

	public static string GetName(int worldNum)
	{
		return worldData[worldNum - 1].Name;
	}

	public static void Reset()
	{
		for (int i = 0; i < worldData.Length; i++)
		{
			worldData[i].Reset();
		}
		for (int j = 0; j < worldRhythms.Length; j++)
		{
			worldRhythms[j].Reset();
		}
		for (int k = 0; k < worldThemes.Length; k++)
		{
			worldThemes[k].Reset();
		}
	}

	public static bool ShouldRecolorWorld(int currentRowNum)
	{
		currentRowNum += GameManager.StartOffset;
		return worldThemes[GameManager.WorldIndex].ShouldChangeTheme(currentRowNum);
	}

	public static void TryChangeGeneratingTheme(int generatingRowNum)
	{
		generatingRowNum += GameManager.StartOffset;
		worldThemes[GameManager.WorldIndex].TryChangeGeneratingTheme(generatingRowNum);
	}

	public static SegmentID GetSegmentChoice(int segmentNum, int rowNum)
	{
		SegmentType type;
		if (GameManager.DebugSegmentType.HasValue)
		{
			type = GameManager.DebugSegmentType.Value;
		}
		else
		{
			float secondsIntoLevel = ((segmentNum != 0) ? TimeOf(rowNum) : 0f);
			bool nextPeriod;
			type = worldRhythms[GameManager.WorldIndex].ChooseSegmentType(secondsIntoLevel, out nextPeriod);
		}
		int difficulty = CurrentWorldData.GetDifficulty(rowNum);
		return new SegmentID(type, difficulty);
	}

	public static float GetVelocity(int rowNum)
	{
		return CurrentWorldData.GetVelocity(rowNum);
	}

	public static float TimeOf(int rowNum)
	{
		return CurrentWorldData.TimeOf(rowNum);
	}

	public static int RowOf(int secondsPlayed)
	{
		return CurrentWorldData.RowOf(secondsPlayed);
	}
}
