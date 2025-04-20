using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
	private enum Command
	{
		Play = 0,
		Stop = 1,
		Pause = 2,
		Unpause = 3,
		FadeOut = 4,
		FadeIn = 5,
		FadeInto = 6,
		FadeOutManual = 7,
		FadeInManual = 8,
		FadeIntoManual = 9,
		SkipTo = 10
	}

	private class TrackPlayer
	{
		private AudioSource firstPlayer;

		private AudioSource secondPlayer;

		private float firstPlayerAudioPlayTime;

		private float secondPlayerAudioPlayTime;

		private bool usingFirstPlayer;

		private AudioSource player
		{
			get
			{
				if (usingFirstPlayer)
				{
					return firstPlayer;
				}
				return secondPlayer;
			}
			set
			{
				if (usingFirstPlayer)
				{
					firstPlayer = value;
				}
				else
				{
					secondPlayer = value;
				}
			}
		}

		public TrackPlayer(GameObject musicPlayerGameObject)
		{
			firstPlayer = musicPlayerGameObject.AddComponent<AudioSource>();
			secondPlayer = musicPlayerGameObject.AddComponent<AudioSource>();
			firstPlayer.playOnAwake = false;
			secondPlayer.playOnAwake = false;
			firstPlayer.loop = true;
			secondPlayer.loop = true;
			usingFirstPlayer = true;
		}

		public bool Play(AudioClip clip, float volume, float pitch, float? skipSeconds)
		{
			if (usingFirstPlayer)
			{
				firstPlayerAudioPlayTime = 0f;
			}
			else
			{
				secondPlayerAudioPlayTime = 0f;
			}
			if (!skipSeconds.HasValue)
			{
				player.time = 0f;
			}
			if (player.isPlaying)
			{
				SwitchPlayer();
			}
			player.clip = clip;
			player.volume = volume;
			player.pitch = pitch;
			player.Play();
			if (skipSeconds.HasValue)
			{
				player.time = skipSeconds.Value;
			}
			return usingFirstPlayer;
		}

		public void Stop(bool playerKey)
		{
			if (playerKey)
			{
				firstPlayerAudioPlayTime = 0f;
				firstPlayer.Stop();
				firstPlayer.time = 0f;
			}
			else
			{
				secondPlayerAudioPlayTime = 0f;
				secondPlayer.Stop();
				secondPlayer.time = 0f;
			}
		}

		public void Pause(bool playerKey)
		{
			if (playerKey)
			{
				firstPlayerAudioPlayTime = firstPlayer.time;
				firstPlayer.Stop();
			}
			else
			{
				secondPlayerAudioPlayTime = secondPlayer.time;
				secondPlayer.Stop();
			}
		}

		public void Unpause(bool playerKey)
		{
			if (playerKey)
			{
				if (!firstPlayer.isPlaying)
				{
					firstPlayer.Play();
					if (firstPlayerAudioPlayTime > 0f)
					{
						firstPlayer.time = firstPlayerAudioPlayTime;
					}
				}
			}
			else if (!secondPlayer.isPlaying)
			{
				secondPlayer.Play();
				if (secondPlayerAudioPlayTime > 0f)
				{
					secondPlayer.time = secondPlayerAudioPlayTime;
				}
			}
		}

		public void SetVolume(bool playerKey, float newVolume)
		{
			if (playerKey)
			{
				firstPlayer.volume = newVolume;
			}
			else
			{
				secondPlayer.volume = newVolume;
			}
		}

		public void SetPitch(bool playerKey, float newPitch)
		{
			if (playerKey)
			{
				firstPlayer.pitch = newPitch;
			}
			else
			{
				secondPlayer.pitch = newPitch;
			}
		}

		public void Reset()
		{
			firstPlayer.Stop();
			secondPlayer.Stop();
			usingFirstPlayer = true;
		}

		private void SwitchPlayer()
		{
			usingFirstPlayer = !usingFirstPlayer;
		}
	}

	private class Track
	{
		private enum State
		{
			Ready = 0,
			Playing = 1,
			FadingIn = 2,
			FadingOut = 3,
			Paused = 4
		}

		private static TrackPlayer player;

		private State state;

		private float fadeTimer;

		private float fadeTime;

		private AudioClip clip;

		private float volume;

		private float pitch;

		private bool playerKey;

		public bool IsPlaying
		{
			get
			{
				return state == State.Playing;
			}
		}

		public bool IsFading
		{
			get
			{
				return IsFadingIn || IsFadingOut;
			}
		}

		public bool IsFadingIn
		{
			get
			{
				return state == State.FadingIn;
			}
		}

		public bool IsFadingOut
		{
			get
			{
				return state == State.FadingOut;
			}
		}

		public bool IsPaused
		{
			get
			{
				return state == State.Paused;
			}
		}

		public bool IsNotPlaying
		{
			get
			{
				return !IsPlaying;
			}
		}

		public bool IsNotFading
		{
			get
			{
				return !IsFading;
			}
		}

		public bool IsNotFadingIn
		{
			get
			{
				return !IsFadingIn;
			}
		}

		public bool IsNotFadingOut
		{
			get
			{
				return !IsFadingOut;
			}
		}

		public bool IsPlayingOrFading
		{
			get
			{
				return IsPlaying || IsFading;
			}
		}

		public bool IsPlayingOrFadingIn
		{
			get
			{
				return IsPlaying || IsFadingIn;
			}
		}

		public bool IsPlayingOrFadingOut
		{
			get
			{
				return IsPlaying || IsFadingOut;
			}
		}

		public bool IsNotPlayingNorFading
		{
			get
			{
				return !IsPlayingOrFading;
			}
		}

		public bool IsNotPlayingNorFadingIn
		{
			get
			{
				return !IsPlayingOrFadingIn;
			}
		}

		public bool IsNotPlayingNorFadingOut
		{
			get
			{
				return !IsPlayingOrFadingOut;
			}
		}

		public string TrackName
		{
			get
			{
				return clip.name;
			}
		}

		public Track(AudioClip clip)
		{
			this.clip = clip;
			volume = 0f;
			pitch = 1f;
			state = State.Ready;
		}

		public static void CreatePlayer(GameObject playerGameObject)
		{
			player = new TrackPlayer(playerGameObject);
		}

		public static void ResetPlayer()
		{
			player.Reset();
		}

		public void Play(float? skipSeconds)
		{
			playerKey = player.Play(clip, globalVolume, globalPitch, skipSeconds);
			state = State.Playing;
		}

		public void Restart(float? skipSeconds)
		{
			Stop();
			Play(skipSeconds);
		}

		public void Stop()
		{
			player.Stop(playerKey);
			state = State.Ready;
		}

		public void Pause()
		{
			player.Pause(playerKey);
			state = State.Paused;
		}

		public void Unpause()
		{
			player.Unpause(playerKey);
			state = State.Playing;
		}

		public void FadeOut(float? fadeTime)
		{
			float num = ((!fadeTime.HasValue) ? 2f : fadeTime.Value);
			if (IsFadingIn)
			{
				float num2 = fadeTimer / this.fadeTime;
				fadeTimer = 1f - num * num2;
				this.fadeTime = num;
			}
			else
			{
				this.fadeTime = num;
				fadeTimer = this.fadeTime;
			}
			state = State.FadingOut;
		}

		public void FadeIn(float? fadeTime)
		{
			float num = ((!fadeTime.HasValue) ? 2f : fadeTime.Value);
			if (IsFadingOut)
			{
				float num2 = fadeTimer / this.fadeTime;
				fadeTimer = 1f - num * num2;
				this.fadeTime = num;
			}
			else
			{
				Play(null);
				this.fadeTime = num;
				fadeTimer = this.fadeTime;
			}
			state = State.FadingIn;
		}

		public bool FadeInto(float? fadeTime)
		{
			bool flag = IsNotPlaying || IsFadingOut;
			if (flag)
			{
				FadeIn(fadeTime);
			}
			return flag;
		}

		public void FadeOutManual()
		{
			Stop();
		}

		public void FadeInManual()
		{
			Play(null);
		}

		public bool FadeIntoManual()
		{
			bool flag = IsNotPlaying || IsFadingOut;
			if (flag)
			{
				FadeInManual();
			}
			return flag;
		}

		public bool UpdateFade()
		{
			fadeTimer -= Time.smoothDeltaTime;
			return EnactUpdateFade(fadeTimer / fadeTime);
		}

		public bool UpdateFadeManual(float updatePercent)
		{
			return true;
		}

		public void RePitch()
		{
			if (IsPlayingOrFading)
			{
				player.SetPitch(playerKey, globalPitch);
			}
		}

		public void ReVolume()
		{
			if (state == State.Playing)
			{
				player.SetVolume(playerKey, globalVolume);
			}
		}

		public void Reset()
		{
			RePitch();
			ReVolume();
		}

		private bool EnactUpdateFade(float fadePercent)
		{
			bool flag = false;
			if (fadePercent <= 0f)
			{
				flag = true;
				fadePercent = 0f;
			}
			else
			{
				fadePercent = FloatAnim.Smooth(fadePercent, true, true);
			}
			if (state == State.FadingIn)
			{
				player.SetVolume(playerKey, globalVolume * (1f - fadePercent));
				if (flag)
				{
					state = State.Playing;
				}
			}
			else if (state == State.FadingOut)
			{
				player.SetVolume(playerKey, globalVolume * fadePercent);
				if (flag)
				{
					Stop();
				}
			}
			else
			{
				Debug.LogWarning(string.Format("MSPL.TRCK: ERROR: Attempt to UpdateFade() on Track {0}, which is currently not fading at all (its current state is: {1})", TrackName, state));
				flag = true;
			}
			return flag;
		}
	}

	private const bool debugsEnabled = true;

	private const bool debugFading = false;

	private const bool debugPlaying = false;

	private const bool debugPending = false;

	private const bool debugAutoFadingStart = false;

	private const string instanceName = "MusicPlayer";

	private const float defaultFadeTime = 2f;

	private static bool isEnabled = true;

	public AudioClip[] music;

	public bool mute;

	private static AudioClip[] receivedTracks;

	private static bool initialized;

	private static bool gameObjectInitialized;

	private static float globalPitch;

	private static float globalVolume;

	private static bool muted;

	private static Track[] tracks;

	private static ArrayUtils.List<int> tracksFadingAuto;

	private static ArrayUtils.List<int> tracksFadingManual;

	private static Command? pendingCommand;

	private static int? pendingCommandTrackNumber;

	private static float? pendingCommandFadeTime;

	private static bool notMuted
	{
		get
		{
			return !muted;
		}
	}

	public static void Initialize()
	{
		Initialize(null, 1f);
	}

	public static void Initialize(float musicVolume)
	{
		Initialize(null, musicVolume);
	}

	public static void Initialize(AudioClip[] musicTracks)
	{
		Initialize(musicTracks, 1f);
	}

	public static void Initialize(AudioClip[] musicTracks, float musicVolume)
	{
		if (!isEnabled)
		{
			return;
		}
		globalPitch = 1f;
		globalVolume = musicVolume;
		if (!initialized)
		{
			if (!gameObjectInitialized)
			{
				receivedTracks = musicTracks;
				GameObject gameObject = new GameObject("MusicPlayer");
				MusicPlayer musicPlayer = gameObject.AddComponent<MusicPlayer>();
			}
			initialized = true;
		}
		else
		{
			for (int i = 0; i < tracks.Length; i++)
			{
				tracks[i].Reset();
			}
		}
	}

	private void Start()
	{
		if (!isEnabled || gameObjectInitialized)
		{
			return;
		}
		if (receivedTracks == null)
		{
			if (MusicNull())
			{
				Debug.LogError("MSPL: ERROR: Music Player had no music specificied, and received no music from an Initialize() call");
			}
		}
		else
		{
			if (!MusicNull())
			{
				Debug.LogError(string.Format("MSPL: ERROR: Music Player had clips specified in its gameObject ('{0}'), but then ALSO received clips from an Initialize() call.  Using received clips", base.name));
			}
			music = receivedTracks;
		}
		Track.CreatePlayer(base.gameObject);
		tracks = new Track[music.Length];
		for (int i = 0; i < tracks.Length; i++)
		{
			tracks[i] = new Track(music[i]);
		}
		tracksFadingAuto = new ArrayUtils.List<int>(tracks.Length);
		tracksFadingManual = new ArrayUtils.List<int>(tracks.Length);
		muted = mute;
		Object.DontDestroyOnLoad(base.gameObject);
		gameObjectInitialized = true;
	}

	private void Update()
	{
		if (!isEnabled)
		{
			return;
		}
		if (tracksFadingAuto.IsNotEmpty)
		{
			for (int i = 0; i < tracksFadingAuto.Length; i++)
			{
				if (tracks[tracksFadingAuto[i]].UpdateFade())
				{
					tracksFadingAuto.RemoveAt(i);
				}
			}
		}
		if (pendingCommand.HasValue)
		{
			if (pendingCommandTrackNumber.HasValue)
			{
				Enact(pendingCommand.Value, pendingCommandTrackNumber.Value, pendingCommandFadeTime, false, false, null);
			}
			else
			{
				EnactGlobal(pendingCommand.Value, pendingCommandFadeTime, false, false, false);
			}
			pendingCommand = null;
			pendingCommandTrackNumber = null;
			pendingCommandFadeTime = null;
		}
	}

	private static void UpdateManual(float fadingPercent)
	{
		if (!isEnabled || !tracksFadingManual.IsNotEmpty)
		{
			return;
		}
		for (int i = 0; i < tracksFadingManual.Length; i++)
		{
			if (tracks[tracksFadingManual[i]].UpdateFadeManual(fadingPercent))
			{
				tracksFadingManual.RemoveAt(i);
			}
		}
	}

	public static void Play(int trackNumber)
	{
		Enact(Command.Play, trackNumber, null, false, false, null);
	}

	public static void Play(int trackNumber, bool resetIfPlaying)
	{
		Enact(Command.Play, trackNumber, null, resetIfPlaying, false, null);
	}

	public static void Play(int trackNumber, float startAtSeconds)
	{
		Enact(Command.Play, trackNumber, null, false, false, startAtSeconds);
	}

	public static void Play(int trackNumber, bool resetIfPlaying, float startAtSeconds)
	{
		Enact(Command.Play, trackNumber, null, resetIfPlaying, false, startAtSeconds);
	}

	public static void Play(int trackNumber, float? startAtSeconds)
	{
		if (startAtSeconds.HasValue)
		{
			Play(trackNumber, startAtSeconds.Value);
		}
		else
		{
			Play(trackNumber);
		}
	}

	public static void Play(int trackNumber, bool resetIfPlaying, float? startAtSeconds)
	{
		if (startAtSeconds.HasValue)
		{
			Play(trackNumber, resetIfPlaying, startAtSeconds.Value);
		}
		else
		{
			Play(trackNumber, resetIfPlaying);
		}
	}

	public static void Pause()
	{
		EnactGlobal(Command.Pause, null, false, false, false);
	}

	public static void Unpause()
	{
		EnactGlobal(Command.Unpause, null, false, false, false);
	}

	public static void Stop()
	{
		EnactGlobal(Command.Stop, null, false, false, false);
	}

	public static void Stop(int trackNumber)
	{
		Enact(Command.Stop, trackNumber, null, false, false, null);
	}

	public static void FadeOut()
	{
		EnactGlobal(Command.FadeOut, null, false, false, false);
	}

	public static void FadeOut(float fadeOutTime)
	{
		EnactGlobal(Command.FadeOut, fadeOutTime, false, false, false);
	}

	public static void FadeOut(int trackNumber)
	{
		Enact(Command.FadeOut, trackNumber, null, false, false, null);
	}

	public static void FadeOut(int trackNumber, float fadeOutTime)
	{
		Enact(Command.FadeOut, trackNumber, null, false, false, null);
	}

	public static void FadeIn(int trackNumber)
	{
		Enact(Command.FadeIn, trackNumber, null, false, false, null);
	}

	public static void FadeIn(int trackNumber, float fadeInTime)
	{
		Enact(Command.FadeIn, trackNumber, fadeInTime, false, false, null);
	}

	public static void FadeIn(int trackNumber, bool resetIfPlaying)
	{
		Enact(Command.FadeIn, trackNumber, null, resetIfPlaying, false, null);
	}

	public static void FadeInto(int trackNumber)
	{
		Enact(Command.FadeInto, trackNumber, null, false, false, null);
	}

	public static void FadeInto(int trackNumber, float fadeInTime)
	{
		Enact(Command.FadeInto, trackNumber, fadeInTime, false, false, null);
	}

	public static void FadeIntoManualStart(int trackNumber)
	{
		Enact(Command.FadeIntoManual, trackNumber, null, false, false, null);
	}

	public static void FadeIntoManualUpdate(float fadePercent)
	{
		UpdateManual(fadePercent);
	}

	public static void FadeIntoManualEnd()
	{
		tracksFadingManual.Reset();
	}

	public static void AdjustPitch(float newPitch)
	{
		if (isEnabled && globalPitch != newPitch)
		{
			globalPitch = newPitch;
			for (int i = 0; i < tracks.Length; i++)
			{
				tracks[i].RePitch();
			}
		}
	}

	public static void SetVolume(float newVolume)
	{
		if (isEnabled && globalVolume != newVolume)
		{
			globalVolume = newVolume;
			for (int i = 0; i < tracks.Length; i++)
			{
				tracks[i].ReVolume();
			}
		}
	}

	public static void Mute()
	{
		muted = true;
		Stop();
	}

	public static void Unmute()
	{
		muted = false;
	}

	public static void Mute(bool on)
	{
		muted = on;
		if (muted)
		{
			Stop();
		}
	}

	private static void Enact(Command command, int trackNumber, float? fadeTime, bool commandBool1, bool commandBool2, float? commandFloat)
	{
		if (!isEnabled)
		{
			return;
		}
		if (gameObjectInitialized)
		{
			if (MathUtils.IndexIsWithin(trackNumber, tracks))
			{
				switch (command)
				{
				case Command.Play:
					if (!notMuted)
					{
						break;
					}
					if (commandBool2)
					{
						for (int j = 0; j < tracks.Length; j++)
						{
							tracks[trackNumber].Stop();
						}
						tracks[trackNumber].Play(commandFloat);
					}
					else if (tracks[trackNumber].IsPlayingOrFadingIn)
					{
						if (commandBool1)
						{
							tracks[trackNumber].Restart(commandFloat);
						}
						else
						{
							Debug.LogWarning(string.Format("MSPL: ERORR: Attempt to Play() track number {0}, which is alreading playing", trackNumber));
						}
					}
					else
					{
						tracks[trackNumber].Play(commandFloat);
					}
					break;
				case Command.Stop:
					if (tracks[trackNumber].IsPlayingOrFading)
					{
						tracks[trackNumber].Stop();
					}
					else
					{
						Debug.LogWarning(string.Format("MSPL: ERORR: Attempt to Stop() track number {0}, which is not currently playing", trackNumber));
					}
					break;
				case Command.FadeOut:
				case Command.FadeIn:
				case Command.FadeInto:
				case Command.FadeOutManual:
				case Command.FadeInManual:
				case Command.FadeIntoManual:
				{
					if (!notMuted && command != Command.FadeOut && command != Command.FadeOutManual)
					{
						break;
					}
					bool flag = true;
					if (tracksFadingAuto.Contains(trackNumber))
					{
						bool flag2 = true;
						string arg = null;
						switch (command)
						{
						case Command.FadeOut:
						case Command.FadeOutManual:
							if (tracks[trackNumber].IsFadingOut)
							{
								arg = "FadeOut()";
								flag2 = true;
								flag = false;
							}
							break;
						case Command.FadeIn:
						case Command.FadeInManual:
							if (tracks[trackNumber].IsFadingIn)
							{
								arg = "FadeIn()";
								flag2 = true;
								flag = false;
							}
							break;
						case Command.FadeInto:
						case Command.FadeIntoManual:
							flag2 = false;
							flag = tracks[trackNumber].IsNotPlayingNorFadingIn;
							break;
						default:
							Debug.LogError(string.Format("MSPL: ERROR: Recevied unhandled Command of {0} in MusicPlay.Enact()'s case statement, nested under FadeOut/FadeIn/FadeInIfStopped case.  Check logic", command));
							flag = false;
							flag2 = false;
							break;
						}
						if (flag2)
						{
							Debug.LogWarning(string.Format("MSPL: ERORR: Attempt to fade {0} track number {1}, which is alreading fading {2}", arg, trackNumber, (!tracks[trackNumber].IsFadingIn) ? "out" : "in"));
							flag = false;
						}
					}
					bool flag3 = false;
					if (flag)
					{
						bool flag4 = true;
						switch (command)
						{
						case Command.FadeOut:
							tracks[trackNumber].FadeOut(fadeTime);
							flag3 = true;
							break;
						case Command.FadeIn:
							tracks[trackNumber].FadeIn(fadeTime);
							flag3 = true;
							break;
						case Command.FadeInto:
							flag4 = tracks[trackNumber].FadeInto(fadeTime);
							flag3 = true;
							break;
						case Command.FadeOutManual:
							tracks[trackNumber].FadeOutManual();
							flag3 = false;
							break;
						case Command.FadeInManual:
							tracks[trackNumber].FadeInManual();
							flag3 = false;
							break;
						case Command.FadeIntoManual:
							flag4 = tracks[trackNumber].FadeIntoManual();
							flag3 = false;
							break;
						default:
							Debug.LogError(string.Format("MSPL: ERROR: Recevied unhandled Command of {0} in MusicPlay.Enact()'s case statement, nested under FadeOut/FadeIn/FadeInIfStopped case.  Check logic", command));
							flag3 = false;
							break;
						}
						if (flag4)
						{
							if (flag3)
							{
								tracksFadingAuto.AddNew(trackNumber);
							}
							else
							{
								tracksFadingManual.AddNew(trackNumber);
							}
						}
					}
					if (command != Command.FadeInto && command != Command.FadeIntoManual)
					{
						break;
					}
					for (int i = 0; i < tracks.Length; i++)
					{
						if (i != trackNumber && tracks[i].IsPlayingOrFadingIn)
						{
							if (flag3)
							{
								tracks[i].FadeOut(fadeTime);
								tracksFadingAuto.AddNew(i);
							}
							else
							{
								tracks[i].FadeOutManual();
								tracksFadingManual.AddNew(i);
							}
						}
					}
					break;
				}
				default:
					Debug.LogError(string.Format("MSPL: ERROR: Recevied unhandled Command of {0} in MusicPlay.Enact()'s case statement.  Check logic", command));
					break;
				}
			}
			else if (tracks == null)
			{
				Debug.LogWarning(string.Format("MSPL: ERORR: Attempt to {0}() track number {1} when NO tracks have been initialized yet", command, trackNumber));
			}
			else
			{
				Debug.LogWarning(string.Format("MSPL: ERORR: Attempt to {0}() track number {1}, which is outside the valid range of tracks (0 to {2})", command, trackNumber, tracks.Length));
			}
		}
		else
		{
			DelayCommand(command, trackNumber, fadeTime);
		}
	}

	private static void EnactGlobal(Command command, float? fadeTime, bool commandBool1, bool commandBool2, bool forceCommand)
	{
		if (!isEnabled && !forceCommand)
		{
			return;
		}
		if (gameObjectInitialized)
		{
			switch (command)
			{
			case Command.Stop:
			{
				for (int l = 0; l < tracks.Length; l++)
				{
					tracks[l].Stop();
				}
				tracksFadingAuto.Reset();
				break;
			}
			case Command.FadeOut:
			{
				for (int j = 0; j < tracks.Length; j++)
				{
					if (tracks[j].IsPlayingOrFadingIn)
					{
						tracks[j].FadeOut(fadeTime);
						tracksFadingAuto.AddNew(j);
					}
				}
				break;
			}
			case Command.Pause:
			{
				for (int k = 0; k < tracks.Length; k++)
				{
					if (tracks[k].IsPlayingOrFading)
					{
						tracks[k].Pause();
					}
				}
				break;
			}
			case Command.Unpause:
			{
				for (int i = 0; i < tracks.Length; i++)
				{
					if (tracks[i].IsPaused)
					{
						tracks[i].Unpause();
					}
				}
				break;
			}
			default:
				Debug.LogError(string.Format("MSPL: ERROR: Recevied unhandled Command of {0} in MusicPlay.Enact()'s affect-all-tracks case statement.  Check logic", command));
				break;
			}
		}
		else
		{
			DelayCommand(command, null, fadeTime);
		}
	}

	private static void DelayCommand(Command commandToDelay, int? trackNumber, float? fadeTime)
	{
		if (pendingCommand.HasValue)
		{
			Debug.LogError(string.Format("MSPL: ERROR: MusicPlayer received command to {0}, after it already received a command to {1} and all before it had finished setting itself up.  Currently MusicPlayer can only store 1 command before it is setup.  Consider extending this functionality", commandToDelay, pendingCommand.Value));
			return;
		}
		pendingCommand = commandToDelay;
		pendingCommandTrackNumber = trackNumber;
		pendingCommandFadeTime = fadeTime;
	}

	private bool MusicNull()
	{
		bool flag = false;
		if (music != null && music.Length > 0)
		{
			for (int i = 0; i < music.Length; i++)
			{
				if (music[i] != null)
				{
					flag = true;
					break;
				}
			}
		}
		return !flag;
	}
}
