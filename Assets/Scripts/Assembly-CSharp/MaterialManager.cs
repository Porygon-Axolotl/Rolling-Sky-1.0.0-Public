using UnityEngine;

public class MaterialManager : MonoBehaviour
{
	public struct TextureSet
	{
		public ArrayUtils.GeniList<Texture> Textures;

		public Texture this[MaterialName materialName]
		{
			get
			{
				int index = texturedMaterials.IndexOf(materialName);
				return Textures[index];
			}
		}

		public TextureSet(params Texture[] textures)
		{
			Textures = new ArrayUtils.GeniList<Texture>();
			for (int i = 0; i < textures.Length; i++)
			{
				Textures.Add(textures[i]);
			}
		}
	}

	public const int FogDistanceDefault = 25;

	public const float SliderBorder = 0.5f;

	public const int TilesPerColor = 80;

	public const int TilesPerColorShift = 5;

	public const float ScoreBrightness = 0.65f;

	public const int FirstColor = 0;

	private const float pickupGloss = 0.05f;

	private const string colorName = "_Color";

	private const string colorNameAlt = "_ColorAlt";

	private const string colorNameR = "_ColorR";

	private const string colorNameG = "_ColorG";

	private const string colorNameB = "_ColorB";

	private const string colorNameA = "_ColorA";

	private const string colorNameF = "_ColorF";

	private const string colorNameD = "_ColorD";

	private const string colorNameS = "_ColorS";

	private const string colorNameSa = "_ColorSa";

	private const string colorNameE = "_ColorE";

	private const string colorNameEAlt = "_ColorEAlt";

	private const string ammountName = "_Ammount";

	private const string specName = "_Specular";

	private const string glossName = "_Gloss";

	private const string alphaName = "_Alpha";

	private static ColorSet[] colorSets = new ColorSet[9]
	{
		new ColorSet(249, 183, 79, 250, 129, 48, 248, 183, 79, 248, 183, 79, 117, 207, 220, 117, 207, 220, 7, 145, 148, 206, 173, 79, 202, 253, 255, 0, 0, 0, 0.25f, 255, 255, 255, 90, 255, 255, 55, 200, 255, 255, 47, 4, 255, 47, 4, 255, 255, 255, 255, 128, 0, 255, 0, 128, 255, 64, 64, 255, 0, 0, 216, 47, 0, 255, 255, 255, 122, 68, 68, 18, 217, 224, 231, 124, 114, 255, 255, 255, 0.3f, 0.1f, 5, 25),
		new ColorSet(238, 238, 255, 234, 134, 191, 241, 211, 242, 241, 211, 242, 235, 134, 192, 235, 134, 192, 130, 55, 126, 65, 0, 120, 202, 253, 255, 65, 0, 120, 0.65f, 255, 255, 255, 100, 255, 255, 204, 68, 255, 125, 76, 171, 175, 131, 255, 255, 255, 255, 255, 192, 255, 138, 84, 169, 210, 145, 255, 188, 132, 235, 4, 27, 255, 255, 255, 255, 122, 68, 68, 65, 0, 120, 215, 114, 239, 255, 255, 255, 1f, 0.1f, 5, 25),
		new ColorSet(104, 216, 103, 44, 163, 124, 132, 255, 130, 132, 255, 130, 54, 181, 152, 54, 181, 152, 25, 99, 91, 58, 129, 96, 202, 253, 255, 9, 64, 51, 0.25f, 255, 255, 255, 255, 255, 0, 255, 164, 50, 255, 98, 9, 255, 83, 19, 255, 255, 255, 255, 128, 0, 255, 0, 128, 255, 64, 64, 255, 0, 0, 255, 121, 9, 255, 255, 222, 115, 76, 56, 0, 87, 67, 60, 190, 255, 255, 255, 255, 1.5f, 0.5f, 5, 25),
		new ColorSet(105, 181, 255, 255, 124, 230, 202, 248, 253, 202, 248, 253, 173, 238, 254, 75, 124, 174, 75, 124, 174, 117, 120, 142, 202, 253, 255, 34, 97, 152, 0.5f, 255, 255, 255, 255, 156, 255, 85, 50, 198, 20, 36, 51, 0, 81, 165, 255, 255, 255, 255, 128, 0, 255, 0, 128, 255, 64, 64, 255, 0, 0, 0, 0, 0, 255, 255, 255, 100, 100, 100, 255, 255, 255, 225, 225, 225, 255, 255, 255, 1.5f, 1f, 5, 45),
		new ColorSet(49, 47, 64, 249, 42, 42, 244, 41, 42, 246, 157, 16, 241, 41, 42, 241, 41, 42, 150, 29, 29, 15, 0, 0, 202, 253, 255, 15, 0, 0, 0.25f, 255, 247, 176, 255, 120, 42, 183, 39, 13, 255, 0, 0, 255, 255, 0, 255, 255, 255, 255, 128, 0, 255, 0, 128, 255, 64, 64, 255, 0, 0, 216, 112, 0, 255, 195, 0, 109, 13, 0, 35, 35, 42, 156, 42, 22, 167, 178, 255, 1f, 1.5f, 10, 25),
		new ColorSet(249, 183, 79, 250, 129, 48, 248, 183, 79, 248, 183, 79, 117, 207, 220, 117, 207, 220, 44, 145, 143, 206, 173, 79, 202, 253, 255, 38, 114, 116, 0.25f, 255, 255, 255, 90, 255, 255, 55, 200, 255, 255, 47, 4, 255, 47, 4, 255, 255, 255, 255, 128, 0, 255, 0, 128, 255, 64, 64, 255, 0, 0, 255, 47, 4, 255, 255, 255, 122, 68, 68, 18, 217, 224, 231, 124, 114, 255, 255, 255, 0.3f, 0.1f, 5, 25),
		new ColorSet(249, 183, 79, 250, 129, 48, 248, 183, 79, 248, 183, 79, 117, 207, 220, 117, 207, 220, 44, 145, 143, 150, 101, 39, 202, 253, 255, 38, 114, 116, 0.25f, 255, 255, 0, 0, 255, 0, 55, 128, 0, 61, 139, 51, 255, 179, 75, 255, 255, 255, 255, 128, 0, 255, 0, 128, 255, 64, 64, 255, 0, 0, 255, 126, 0, 255, 253, 249, 112, 28, 28, 203, 83, 49, 255, 97, 26, 255, 179, 75, 1f, 0.25f, 10, 25),
		new ColorSet(249, 183, 79, 250, 129, 48, 241, 211, 242, 241, 211, 242, 235, 134, 192, 235, 134, 192, 130, 55, 126, 38, 114, 116, 202, 253, 255, 38, 114, 116, 0.25f, 255, 255, 255, 90, 255, 255, 55, 200, 255, 255, 56, 15, 255, 179, 75, 255, 255, 255, 255, 128, 0, 255, 0, 128, 255, 64, 64, 255, 0, 0, 255, 126, 0, 255, 253, 249, 112, 28, 28, 203, 83, 49, 255, 97, 26, 255, 179, 75, 1f, 0.25f, 10, 25),
		new ColorSet(249, 183, 79, 250, 129, 48, 202, 248, 253, 202, 248, 253, 173, 238, 254, 173, 238, 254, 75, 124, 174, 182, 31, 102, 202, 253, 255, 38, 114, 116, 0.25f, 255, 255, 255, 90, 255, 255, 55, 200, 255, 255, 56, 15, 255, 179, 75, 255, 255, 255, 255, 128, 0, 255, 0, 128, 255, 64, 64, 255, 0, 0, 255, 248, 0, 255, 253, 249, 112, 28, 28, 11, 11, 10, 11, 11, 10, 255, 163, 163, 1f, 0.25f, 10, 25)
	};

	private static ArrayUtils.Array<MaterialName> texturedMaterials = new ArrayUtils.Array<MaterialName>(MaterialName.BallQuad, MaterialName.BallTrail, MaterialName.Pickup, MaterialName.General, MaterialName.Fragile, MaterialName.FragileActive, MaterialName.Mover, MaterialName.MoverAuto, MaterialName.Enemy, MaterialName.Laser, MaterialName.Portal, MaterialName.Finish, MaterialName.Midground, MaterialName.Background);

	public Material ballMaterial;

	public Material ballQuadMaterial;

	public Material ballTrailMaterial;

	public Material ballInnerMaterial;

	public Material pickupMaterial;

	public Material generalMasterMaterial;

	public Material fragileMasterMaterial;

	public Material fragileActiveMasterMaterial;

	public Material moverMasterMaterial;

	public Material moverAutoMasterMaterial;

	public Material enemyMasterMaterial;

	public Material midgroundMaterial;

	public Material backgroundMaterial;

	public Material finishMaterial;

	public Material laserMaterial;

	public Material portalMaterial;

	public Material hudForegroundMaterial;

	public Material hudBackgroundMaterial;

	public Material inGameHudMaterial;

	public Material menuOverlayMaterial;

	public Material nextWorldRevealMaterial;

	public Shader unlitShader;

	public Shader unlitBallQuadShader;

	public Shader litBallGeoShader;

	public Shader litPickupShader;

	public Shader litEnemyShader;

	public Shader fullPickupShader;

	public Shader transparentShader;

	public Shader backgroundShaderNormal;

	public Shader backgroundShaderLerped;

	public Shader midgroundShaderNormal;

	public Shader midgroundShaderLerped;

	public Shader fullBallInnerShader;

	public Shader basicBallInnerShader;

	public Texture[] WorldBallQuadTextures;

	public Texture[] WorldGeneralTextures;

	public Texture[] WorldFragileTextures;

	public Texture[] WorldFragileActiveTextures;

	public Texture[] WorldMoverTextures;

	public Texture[] WorldMoverAutoTextures;

	public Texture[] WorldEnemyTextures;

	public Texture[] WorldBackgroundTextures;

	public Texture[] WorldMenuImages;

	private static ArrayUtils.SmartDict<MaterialName, ShaderPair> Shaders;

	private static bool isSetup;

	private static bool transitioning;

	private static int lastColorNum;

	private static int currentColorNum;

	private static MathUtils.Index colorNumNext;

	private static TextMesh score;

	private static Light mainLight;

	private static ShaderPair backgroundShader;

	private static ShaderPair midgroundShader;

	private static bool usingLitShaders;

	private static bool usingGeoBall;

	private static bool usingFullShaders;

	public static ArrayUtils.SmartDict<MaterialName, Material> Materials { get; private set; }

	public static ArrayUtils.Array<TextureSet> WorldTextures { get; private set; }

	public static ArrayUtils.Array<Texture> WorldMenuTextures { get; private set; }

	public static void Initialize(int startingColorNum)
	{
		Initialize(startingColorNum, null);
	}

	public static void Initialize(int startingColorNum, Transform scoreTextTransform)
	{
		if (!isSetup)
		{
			Debug.LogWarning("MTMN: ERROR: Attempt to Initialize() MaterialManager BEFORE it was setup in this scene");
		}
		currentColorNum = startingColorNum - 1;
		colorNumNext = new MathUtils.Index(colorSets.Length, currentColorNum);
		if (scoreTextTransform != null)
		{
			score = scoreTextTransform.GetComponent<TextMesh>();
		}
		mainLight = Camera.main.GetComponentInChildren<Light>();
		EndRecolor();
		Materials[MaterialName.Ball].SetFloat("_Ammount", 0f);
	}

	private void OnEnable()
	{
		TrySetup();
	}

	private void TrySetup()
	{
		if (isSetup)
		{
			return;
		}
		Materials = new ArrayUtils.SmartDict<MaterialName, Material>();
		Materials.Add(MaterialName.General, generalMasterMaterial);
		Materials.Add(MaterialName.Fragile, fragileMasterMaterial);
		Materials.Add(MaterialName.FragileActive, fragileActiveMasterMaterial);
		Materials.Add(MaterialName.Mover, moverMasterMaterial);
		Materials.Add(MaterialName.MoverAuto, moverAutoMasterMaterial);
		Materials.Add(MaterialName.Enemy, enemyMasterMaterial);
		Materials.Add(MaterialName.Midground, midgroundMaterial);
		Materials.Add(MaterialName.Background, backgroundMaterial);
		Materials.Add(MaterialName.Pickup, pickupMaterial);
		Materials.Add(MaterialName.Finish, finishMaterial);
		Materials.Add(MaterialName.Laser, laserMaterial);
		Materials.Add(MaterialName.Portal, portalMaterial);
		Materials.Add(MaterialName.HudForeground, hudForegroundMaterial);
		Materials.Add(MaterialName.HudBackground, hudBackgroundMaterial);
		Materials.Add(MaterialName.InGameHud, inGameHudMaterial);
		Materials.Add(MaterialName.MenuOverlay, menuOverlayMaterial);
		Materials.Add(MaterialName.NextWorldReveal, nextWorldRevealMaterial);
		Materials.Add(MaterialName.Ball, ballMaterial);
		Materials.Add(MaterialName.BallQuad, ballQuadMaterial);
		Materials.Add(MaterialName.BallTrail, ballTrailMaterial);
		Materials.Add(MaterialName.BallInner, ballInnerMaterial);
		WorldMenuTextures = new ArrayUtils.Array<Texture>(WorldMenuImages);
		WorldTextures = new ArrayUtils.Array<TextureSet>(WorldGeneralTextures.Length);
		for (int i = 0; i < WorldTextures.Length; i++)
		{
			WorldTextures[i] = new TextureSet(WorldBallQuadTextures[i], WorldEnemyTextures[i], WorldFragileTextures[i], WorldGeneralTextures[i], WorldFragileTextures[i], WorldFragileActiveTextures[i], WorldMoverTextures[i], WorldMoverAutoTextures[i], WorldEnemyTextures[i], WorldEnemyTextures[i], WorldEnemyTextures[i], WorldFragileActiveTextures[i], WorldFragileTextures[i], WorldBackgroundTextures[i]);
		}
		usingLitShaders = DeviceQualityChecker.QualityIsNotPour();
		usingGeoBall = DeviceQualityChecker.QualityIsNotPour();
		usingFullShaders = DeviceQualityChecker.QualityIsHigh();
		if (usingLitShaders)
		{
			Materials[MaterialName.Enemy].shader = litEnemyShader;
			if (usingFullShaders)
			{
				Materials[MaterialName.Fragile].shader = transparentShader;
				Materials[MaterialName.FragileActive].shader = unlitShader;
				Materials[MaterialName.Finish].shader = transparentShader;
				Materials[MaterialName.BallInner].shader = fullBallInnerShader;
				Materials[MaterialName.Pickup].shader = fullPickupShader;
				Materials[MaterialName.Pickup].SetFloat("_Gloss", 0.05f);
			}
			else
			{
				Materials[MaterialName.Pickup].shader = litPickupShader;
				Materials[MaterialName.Fragile].shader = transparentShader;
				Materials[MaterialName.FragileActive].shader = unlitShader;
				Materials[MaterialName.Finish].shader = transparentShader;
				Materials[MaterialName.BallInner].shader = basicBallInnerShader;
			}
		}
		else
		{
			Materials[MaterialName.Pickup].shader = unlitShader;
			Materials[MaterialName.Enemy].shader = unlitShader;
			Materials[MaterialName.Fragile].shader = unlitShader;
			Materials[MaterialName.FragileActive].shader = unlitShader;
			Materials[MaterialName.Finish].shader = unlitShader;
			Materials[MaterialName.BallInner].shader = basicBallInnerShader;
		}
		if (usingGeoBall)
		{
			Materials[MaterialName.Ball].shader = litBallGeoShader;
		}
		else
		{
			Materials[MaterialName.BallQuad].shader = unlitBallQuadShader;
		}
		backgroundShader = new ShaderPair(backgroundShaderNormal, backgroundShaderLerped);
		midgroundShader = new ShaderPair(midgroundShaderNormal, midgroundShaderLerped);
		isSetup = true;
	}

	public static int StartRecolor()
	{
		return StartRecolor(true);
	}

	public static int StartRecolor(bool toNext)
	{
		if (transitioning)
		{
			lastColorNum = currentColorNum;
		}
		int newColorNum = ((!toNext) ? colorNumNext.GetPrevious() : colorNumNext.GetNext());
		StartRecolorTo(newColorNum);
		return currentColorNum;
	}

	public static int StartRecolorTo(int newColorNum)
	{
		if (transitioning)
		{
			lastColorNum = currentColorNum;
		}
		currentColorNum = newColorNum;
		transitioning = true;
		return currentColorNum;
	}

	public static void Recolor(float transitionPercent, bool inversePercent)
	{
		Recolor(1f - transitionPercent);
	}

	public static void Recolor(float transitionPercent)
	{
		if (transitionPercent >= 1f)
		{
			EndRecolor();
		}
		else
		{
			EnactPartialRecolor(transitionPercent);
		}
	}

	public static void EndRecolor()
	{
		lastColorNum = currentColorNum;
		ColorSet colors = colorSets[currentColorNum];
		EnactRecolor(colors);
		transitioning = false;
	}

	public static void StartLerping(int firstTextureIndex, int secondTextureIndex)
	{
		Materials[MaterialName.Background].shader = backgroundShader.Alternate;
		Materials[MaterialName.Midground].shader = midgroundShader.Alternate;
		UpdateLerping(0f);
		Materials[MaterialName.Background].SetTexture("_MainTex", WorldTextures[firstTextureIndex][MaterialName.Background]);
		Materials[MaterialName.Background].SetTexture("_AltTex", WorldTextures[secondTextureIndex][MaterialName.Background]);
		Materials[MaterialName.Midground].SetTexture("_MainTex", WorldTextures[firstTextureIndex][MaterialName.Midground]);
		Materials[MaterialName.Midground].SetTexture("_AltTex", WorldTextures[secondTextureIndex][MaterialName.Midground]);
	}

	public static void UpdateLerping(float lerpPercent)
	{
		Materials[MaterialName.Background].SetFloat("_Ammount", lerpPercent);
		Materials[MaterialName.Midground].SetFloat("_Ammount", lerpPercent);
	}

	public static void EndLerping(int? endingTextureIndex)
	{
		Materials[MaterialName.Background].shader = backgroundShader.Normal;
		Materials[MaterialName.Midground].shader = midgroundShader.Normal;
		if (endingTextureIndex.HasValue)
		{
			Materials[MaterialName.Background].mainTexture = WorldTextures[endingTextureIndex.Value][MaterialName.Background];
			Materials[MaterialName.Midground].mainTexture = WorldTextures[endingTextureIndex.Value][MaterialName.Midground];
		}
	}

	public static Color GetPrimaryColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].Primary.color;
	}

	public static Color GetSecondaryColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].Secondary.color;
	}

	public static Color GetGuiLightColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].GuiLight.color;
	}

	public static Color GetGuiLightAltColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].GuiLightAlt.color;
	}

	public static Color GetGuiMidColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].GuiMid.color;
	}

	public static Color GetGuiMidAltColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].GuiMidAlt.color;
	}

	public static Color GetGuiDarkColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].GuiDark.color;
	}

	public static Color GetGuiButtonTextColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].GuiButtonText.color;
	}

	public static Color GetGuiRevealColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].GuiReveal.color;
	}

	public static Color GetGuiOverlayColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].GuiOverlay.color;
	}

	public static float GetGuiOverlayAlpha(int WorldNum)
	{
		return colorSets[WorldNum - 1].GuiOverlayAlpha;
	}

	public static Color GetEnergyHighColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].EnergyHigh.color;
	}

	public static Color GetEnergyMidHighColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].EnergyMidHigh.color;
	}

	public static Color GetEnergyColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].Energy.color;
	}

	public static Color GetEnergyMidLowColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].EnergyMidLow.color;
	}

	public static Color GetEnergyLowColor(int WorldNum)
	{
		return colorSets[WorldNum - 1].EnergyLow.color;
	}

	public static Color[] GetEnergyColors(int WorldNum)
	{
		return colorSets[WorldNum - 1].EnergyColors;
	}

	public static Color[] GetEnergyParticleColors(int WorldNum)
	{
		return colorSets[WorldNum - 1].EnergyParticles;
	}

	public static Color[] GetPickupParticleColors(int WorldNum)
	{
		return colorSets[WorldNum - 1].PickupParticles;
	}

	public static Color GetPrimaryColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].Primary.color;
	}

	public static Color GetSecondaryColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].Secondary.color;
	}

	public static Color GetGuiLightColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].GuiLight.color;
	}

	public static Color GetGuiLightAltColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].GuiLightAlt.color;
	}

	public static Color GetGuiMidColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].GuiMid.color;
	}

	public static Color GetGuiMidAltColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].GuiMidAlt.color;
	}

	public static Color GetGuiDarkColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].GuiDark.color;
	}

	public static Color GetGuiButtonTextColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].GuiButtonText.color;
	}

	public static Color GetGuiRevealColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].GuiReveal.color;
	}

	public static Color GetGuiOverlayColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].GuiOverlay.color;
	}

	public static float GetGuiOverlayAlpha()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].GuiOverlayAlpha;
	}

	public static Color GetEnergyHighColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].EnergyHigh.color;
	}

	public static Color GetEnergyMidHighColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].EnergyMidHigh.color;
	}

	public static Color GetEnergyColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].Energy.color;
	}

	public static Color GetEnergyMidLowColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].EnergyMidLow.color;
	}

	public static Color GetEnergyLowColor()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].EnergyLow.color;
	}

	public static Color[] GetEnergyColors()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].EnergyColors;
	}

	public static Color[] GetEnergyParticleColors()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].EnergyParticles;
	}

	public static Color[] GetPickupParticleColors()
	{
		return colorSets[LevelDesigner.CurrentWorldThemeIndex].PickupParticles;
	}

	public static Color GetNextPrimaryColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].Primary.color;
	}

	public static Color GetNextSecondaryColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].Secondary.color;
	}

	public static Color GetNextGuiLightColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].GuiLight.color;
	}

	public static Color GetNextGuiLightAltColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].GuiLightAlt.color;
	}

	public static Color GetNextGuiMidColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].GuiMid.color;
	}

	public static Color GetNextGuiMidAltColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].GuiMidAlt.color;
	}

	public static Color GetNextGuiDarkColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].GuiDark.color;
	}

	public static Color GetNextGuiButtonTextColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].GuiButtonText.color;
	}

	public static Color GetNextGuiRevealColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].GuiReveal.color;
	}

	public static Color GetNextGuiOverlayColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].GuiOverlay.color;
	}

	public static float GetNextGuiOverlayAlpha()
	{
		return colorSets[GameManager.GetNextWorldIndex()].GuiOverlayAlpha;
	}

	public static Color GetNextEnergyHighColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].EnergyHigh.color;
	}

	public static Color GetNextEnergyMidHighColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].EnergyMidHigh.color;
	}

	public static Color GetNextEnergyColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].Energy.color;
	}

	public static Color GetNextEnergyMidLowColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].EnergyMidLow.color;
	}

	public static Color GetNextEnergyLowColor()
	{
		return colorSets[GameManager.GetNextWorldIndex()].EnergyLow.color;
	}

	public static Color[] GetNextEnergyColors()
	{
		return colorSets[GameManager.GetNextWorldIndex()].EnergyColors;
	}

	public static Color[] GetNextEnergyParticleColors()
	{
		return colorSets[GameManager.GetNextWorldIndex()].EnergyParticles;
	}

	public static Color[] GetNextPickupParticleColors()
	{
		return colorSets[GameManager.GetNextWorldIndex()].PickupParticles;
	}

	private static void EnactRecolor(ColorSet colors)
	{
		int textureIndex = MathUtils.Indexed(currentColorNum, WorldTextures.Length);
		ApplyTexture(textureIndex);
		if (usingLitShaders)
		{
			if (usingFullShaders)
			{
				Materials[MaterialName.Pickup].SetColor("_ColorD", colors.PickupDiff.color);
				Materials[MaterialName.Pickup].SetColor("_ColorS", colors.PickupSpec.color);
				Materials[MaterialName.Pickup].SetColor("_ColorR", colors.PickupRefl.color);
				Materials[MaterialName.BallInner].SetColor("_ColorE", colors.EnergyHigh.color);
				Materials[MaterialName.BallInner].SetColor("_ColorEAlt", colors.EnergyLow.color);
			}
			else
			{
				Materials[MaterialName.Pickup].SetColor("_Color", colors.PickupSpec.color);
				Materials[MaterialName.Pickup].SetFloat("_Specular", colors.EnemySpecAmnt);
				Materials[MaterialName.Pickup].SetFloat("_Gloss", colors.EnemyGloss);
			}
			if (usingGeoBall)
			{
				Materials[MaterialName.Ball].SetColor("_ColorD", colors.BallDiff.color);
				Materials[MaterialName.Ball].SetColor("_ColorS", colors.BallSpec.color);
				Materials[MaterialName.Ball].SetColor("_ColorSa", colors.BallSpecAmbi.color);
			}
			Materials[MaterialName.Enemy].SetColor("_Color", colors.EnemySpec.color);
			Materials[MaterialName.Enemy].SetFloat("_Specular", colors.EnemySpecAmnt);
			Materials[MaterialName.Enemy].SetFloat("_Gloss", colors.EnemyGloss);
			Materials[MaterialName.Midground].SetColor("_ColorF", colors.BackgroundFog.color);
		}
		Materials[MaterialName.BallInner].SetColor("_Color", colors.Energy.color);
		Materials[MaterialName.Ball].SetColor("_ColorF", colors.BackgroundFog.color);
		Camera.main.backgroundColor = colors.Camera.color;
		RenderSettings.fogColor = colors.ForegroundFog.color;
		RenderSettings.fogStartDistance = colors.FogStart;
		RenderSettings.fogEndDistance = colors.FogEnd;
		Color color = colors.Score.color;
		if (score != null)
		{
			score.color = color;
		}
		Materials[MaterialName.InGameHud].SetColor("_Color", color);
		Materials[MaterialName.NextWorldReveal].SetColor("_Color", colors.GuiReveal.color);
		Materials[MaterialName.MenuOverlay].SetColor("_Color", colors.GuiOverlay.color);
		Materials[MaterialName.MenuOverlay].SetFloat("_Alpha", colors.GuiOverlayAlpha);
	}

	private static void EnactPartialRecolor(float transitionPercent)
	{
		ColorSet colorSet = colorSets[lastColorNum];
		ColorSet colorSet2 = colorSets[currentColorNum];
		if (usingLitShaders)
		{
			if (usingFullShaders)
			{
				Materials[MaterialName.Pickup].SetColor("_ColorD", colorSet.PickupDiff.GetBlended(colorSet2.PickupDiff, transitionPercent));
				Materials[MaterialName.Pickup].SetColor("_ColorS", colorSet.PickupSpec.GetBlended(colorSet2.PickupSpec, transitionPercent));
				Materials[MaterialName.Pickup].SetColor("_ColorR", colorSet.PickupRefl.GetBlended(colorSet2.PickupRefl, transitionPercent));
				Materials[MaterialName.BallInner].SetColor("_ColorE", colorSet.EnergyHigh.GetBlended(colorSet2.EnergyHigh, transitionPercent));
				Materials[MaterialName.BallInner].SetColor("_ColorEAlt", colorSet.EnergyLow.GetBlended(colorSet2.EnergyLow, transitionPercent));
			}
			else
			{
				Materials[MaterialName.Pickup].SetColor("_Color", colorSet.PickupSpec.GetBlended(colorSet2.PickupSpec, transitionPercent));
				Materials[MaterialName.Pickup].SetFloat("_Specular", MathUtils.Lerp(colorSet.EnemySpecAmnt, colorSet2.EnemySpecAmnt, transitionPercent));
				Materials[MaterialName.Pickup].SetFloat("_Gloss", MathUtils.Lerp(colorSet.EnemyGloss, colorSet2.EnemyGloss, transitionPercent));
			}
			if (usingGeoBall)
			{
				Materials[MaterialName.Ball].SetColor("_ColorD", colorSet.BallDiff.GetBlended(colorSet2.BallDiff, transitionPercent));
				Materials[MaterialName.Ball].SetColor("_ColorS", colorSet.BallSpec.GetBlended(colorSet2.BallSpec, transitionPercent));
				Materials[MaterialName.Ball].SetColor("_ColorSa", colorSet.BallSpecAmbi.GetBlended(colorSet2.BallSpecAmbi, transitionPercent));
			}
			Materials[MaterialName.Enemy].SetColor("_Color", colorSet.EnemySpec.GetBlended(colorSet2.EnemySpec, transitionPercent));
			Materials[MaterialName.Enemy].SetFloat("_Specular", MathUtils.Lerp(colorSet.EnemySpecAmnt, colorSet2.EnemySpecAmnt, transitionPercent));
			Materials[MaterialName.Enemy].SetFloat("_Gloss", MathUtils.Lerp(colorSet.EnemyGloss, colorSet2.EnemyGloss, transitionPercent));
			Materials[MaterialName.Midground].SetColor("_ColorF", colorSet.BackgroundFog.GetBlended(colorSet2.BackgroundFog, transitionPercent));
		}
		Materials[MaterialName.BallInner].SetColor("_Color", colorSet.Energy.GetBlended(colorSet2.Energy, transitionPercent));
		Materials[MaterialName.Ball].SetColor("_ColorF", colorSet.BackgroundFog.GetBlended(colorSet2.BackgroundFog, transitionPercent));
		Camera.main.backgroundColor = colorSet.Camera.GetBlended(colorSet2.Camera, transitionPercent);
		RenderSettings.fogColor = colorSet.ForegroundFog.GetBlended(colorSet2.ForegroundFog, transitionPercent);
		RenderSettings.fogStartDistance = MathUtils.Lerp(colorSet.FogStart, colorSet2.FogStart, transitionPercent);
		RenderSettings.fogEndDistance = MathUtils.Lerp(colorSet.FogEnd, colorSet2.FogEnd, transitionPercent);
		Color blended = colorSet.Score.GetBlended(colorSet2.Score, transitionPercent);
		if (score != null)
		{
			score.color = blended;
		}
		Materials[MaterialName.InGameHud].SetColor("_Color", blended);
		Materials[MaterialName.NextWorldReveal].SetColor("_Color", colorSet.GuiReveal.GetBlended(colorSet2.GuiReveal, transitionPercent));
		Materials[MaterialName.MenuOverlay].SetColor("_Color", colorSet.GuiOverlay.GetBlended(colorSet2.GuiOverlay, transitionPercent));
		Materials[MaterialName.MenuOverlay].SetFloat("_Alpha", MathUtils.Lerp(colorSet.GuiOverlayAlpha, colorSet2.GuiOverlayAlpha, transitionPercent));
	}

	private static void ApplyTexture(int textureIndex)
	{
		for (int i = 0; i < texturedMaterials.Length; i++)
		{
			Materials[texturedMaterials[i]].mainTexture = WorldTextures[textureIndex][texturedMaterials[i]];
		}
	}
}
