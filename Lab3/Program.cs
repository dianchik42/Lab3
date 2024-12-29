using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
	[Serializable]
	public class GameState
	{
		public char[,] DungeonMap;
		public int PlayerX;
		public int PlayerY;
		public int TreasureCount;
		public int MoveCount;
		public int[,] Enemies;
		public int MapSize;
		public int TotalTreasures;
		public int EnemyCount;
	}
	public class Program
	{
		public static char[,] dungeonMap;
		static int playerX, playerY;
		public static int treasureCount = 0;
		public static int moveCount = 0;
		static Random random = new Random();
		static int enemyCount = 3;
		public static int[,] enemies;
		static int mapSize = 10;
		static int totalTreasures = 5;
		public static string saveFilePath = "savegame.txt";
		public static bool isGameInitialized = false;
		static bool exitGame = false;
		public static bool isTestMode = false;

		public static void ResetState()
		{
			dungeonMap = null;
			playerX = 0;
			playerY = 0;
			treasureCount = 0;
			moveCount = 0;
			enemies = null;
			isGameInitialized = false;
			exitGame = false;
		}
		public static char[,] GetDungeonMap() => dungeonMap;
		public static int GetMapSize() => mapSize;


		static void Main(string[] args)
		{
			ShowMenu();
		}

		static void ShowMenu()
		{
			while (!exitGame)
			{
				Console.Clear();
				Console.WriteLine("Dungeon Crawler");
				Console.WriteLine("1. Новая игра");
				Console.WriteLine("2. Продолжить игру");
				Console.WriteLine("3. Настройки");
				Console.WriteLine("4. Сохранить игру");
				Console.WriteLine("5. Выход (Esc)");

				ConsoleKey key = Console.ReadKey(true).Key;

				switch (key)
				{
					case ConsoleKey.D1:
						InitializeGame();
						GameLoop();
						break;
					case ConsoleKey.D2:
						if (File.Exists(saveFilePath))
						{
							LoadGame();
							GameLoop();
						}
						else
						{
							Console.WriteLine("Нет сохраненной игры. Нажмите любую клавишу, чтобы вернуться в меню.");
							Console.ReadKey();
						}
						break;
					case ConsoleKey.D3:
						Settings();
						break;
					case ConsoleKey.D4:
						if (isGameInitialized)
						{
							SaveGame();
						}
						else
						{
							Console.WriteLine("Игра не была начата. Нечего сохранять.");
							Console.ReadKey();
						}
						break;
					case ConsoleKey.D5:
					case ConsoleKey.Escape:
						Console.WriteLine("Выход из игры...");
						exitGame = true;
						return;
					default:
						Console.WriteLine("Неверный выбор. Попробуйте снова.");
						break;
				}
			}
		}

		static void Settings()
		{
			Console.Clear();
			Console.WriteLine("Настройки");
			Console.WriteLine("1. Установить сложность (количество врагов)");
			Console.WriteLine("2. Изменить размер карты");
			Console.WriteLine("3. Управление: используйте только клавиши WASD для перемещения, Esc для выхода");
			Console.WriteLine("4. Вернуться в меню");

			ConsoleKey key = Console.ReadKey(true).Key;

			switch (key)
			{
				case ConsoleKey.D1:
					Console.Write("Введите количество врагов: ");
					if (int.TryParse(Console.ReadLine(), out int newEnemyCount) && newEnemyCount > 0)
					{
						enemyCount = newEnemyCount;
						Console.WriteLine("Сложность обновлена.");
					}
					else
					{
						Console.WriteLine("Неверное значение.");
					}
					break;
				case ConsoleKey.D2:
					Console.Write("Введите размер карты: ");
					if (int.TryParse(Console.ReadLine(), out int newMapSize) && newMapSize >= 5)
					{
						mapSize = newMapSize;
						totalTreasures = newMapSize / 2;
						Console.WriteLine("Размер карты обновлен.");
					}
					else
					{
						Console.WriteLine("Неверное значение.");
					}
					break;
				case ConsoleKey.D3:
					Console.WriteLine("Настройка управления: Используйте только клавиши WASD для перемещения и Esc для выхода.");
					Console.ReadKey();
					break;
				case ConsoleKey.D4:
				case ConsoleKey.Escape:
					return;
				default:
					Console.WriteLine("Неверный выбор.");
					break;
			}

			Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню.");
			Console.ReadKey();
		}

		public static void InitializeGame()
		{
			dungeonMap = new char[mapSize, mapSize];
			for (int i = 0; i < mapSize; i++)
			{
				for (int j = 0; j < mapSize; j++)
				{
					dungeonMap[i, j] = '.';
				}
			}

			// Размещение игрока
			playerX = random.Next(0, mapSize);
			playerY = random.Next(0, mapSize);
			dungeonMap[playerX, playerY] = 'P';

			// Размещение сокровищ
			for (int i = 0; i < totalTreasures; i++)
			{
				int treasureX, treasureY;
				do
				{
					treasureX = random.Next(0, mapSize);
					treasureY = random.Next(0, mapSize);
				} while (dungeonMap[treasureX, treasureY] != '.');

				dungeonMap[treasureX, treasureY] = 'T';
			}

			// Размещение врагов
			enemies = new int[enemyCount, 2];
			for (int i = 0; i < enemyCount; i++)
			{
				int enemyX, enemyY;
				do
				{
					enemyX = random.Next(0, mapSize);
					enemyY = random.Next(0, mapSize);
				} while (dungeonMap[enemyX, enemyY] != '.');

				enemies[i, 0] = enemyX;
				enemies[i, 1] = enemyY;
				dungeonMap[enemyX, enemyY] = 'E';
			}

			isGameInitialized = true;
		}

		static void GameLoop()
		{
			while (true)
			{
				RenderMap();
				HandleInput();
				if (exitGame) return;

				UpdateEnemies();

				if (treasureCount == totalTreasures)
				{
					Console.WriteLine("Поздравляем, вы нашли все сокровища! Победа!");
					ResetGameState();
					break;
				}
			}

			Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню.");
			Console.ReadKey();
			ShowMenu();
		}

		static void RenderMap()
		{
			Console.Clear();
			if (dungeonMap == null)
			{
				return;
			}

			for (int i = 0; i < mapSize; i++)
			{
				for (int j = 0; j < mapSize; j++)
				{
					Console.Write(dungeonMap[i, j] + " ");
				}
				Console.WriteLine();
			}
			Console.WriteLine($"Ходы: {moveCount} | Найдено сокровищ: {treasureCount}/{totalTreasures}");
		}

		static void HandleInput()
		{
			ConsoleKey key = Console.ReadKey(true).Key;
			int newX = playerX;
			int newY = playerY;

			switch (key)
			{
				case ConsoleKey.W:
					newX--;
					break;
				case ConsoleKey.S:
					newX++;
					break;
				case ConsoleKey.A:
					newY--;
					break;
				case ConsoleKey.D:
					newY++;
					break;
				case ConsoleKey.Escape:
					Console.WriteLine("Игра приостановлена. Возврат в меню...");
					ShowMenu();
					return;
				default:
					Console.WriteLine("Некорректная клавиша. Используйте только WASD для движения.");
					return;
			}

			if (newX >= 0 && newX < mapSize && newY >= 0 && newY < mapSize)
			{
				if (dungeonMap[newX, newY] == 'T')
				{
					Console.WriteLine("Вы нашли сокровище!");
					treasureCount++;
				}
				else if (dungeonMap[newX, newY] == 'E')
				{
					Console.WriteLine("Вы столкнулись с врагом! Игра окончена.");
					Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню.");
					ResetGameState();
					Console.ReadKey();
					ShowMenu();
					return;
				}

				dungeonMap[playerX, playerY] = '.';
				playerX = newX;
				playerY = newY;
				dungeonMap[playerX, playerY] = 'P';

				moveCount++;
			}
		}

		static void UpdateEnemies()
		{
			for (int i = 0; i < enemyCount; i++)
			{
				int enemyX = enemies[i, 0];
				int enemyY = enemies[i, 1];

				dungeonMap[enemyX, enemyY] = '.';

				int direction = random.Next(4);
				switch (direction)
				{
					case 0:
						enemyX = Math.Max(0, enemyX - 1);
						break;
					case 1:
						enemyX = Math.Min(mapSize - 1, enemyX + 1);
						break;
					case 2:
						enemyY = Math.Max(0, enemyY - 1);
						break;
					case 3:
						enemyY = Math.Min(mapSize - 1, enemyY + 1);
						break;
				}

				if (dungeonMap[enemyX, enemyY] == '.' || dungeonMap[enemyX, enemyY] == 'P')
				{
					enemies[i, 0] = enemyX;
					enemies[i, 1] = enemyY;
				}

				dungeonMap[enemies[i, 0], enemies[i, 1]] = 'E';

				if (enemies[i, 0] == playerX && enemies[i, 1] == playerY)
				{
					Console.WriteLine("Враг поймал вас! Игра окончена.");
					Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню.");
					ResetGameState();
					Console.ReadKey();
					ShowMenu();
					return;
				}
			}
		}

		public static void SaveGame(string filePath = null)
		{
			Console.WriteLine("Start Save/Начало сохранения...");
			filePath ??= saveFilePath;
			GameState state = new GameState
			{
				DungeonMap = dungeonMap,
				PlayerX = playerX,
				PlayerY = playerY,
				TreasureCount = treasureCount,
				MoveCount = moveCount,
				Enemies = enemies,
				MapSize = mapSize,
				TotalTreasures = totalTreasures,
				EnemyCount = enemyCount
			};

			IFormatter formatter = new BinaryFormatter();
			using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			{
				formatter.Serialize(stream, state);
			}

			Console.WriteLine("Game was saved/Игра сохранена.");
			if (!isTestMode) Console.ReadKey();
		}


		public static void LoadGame(string filePath = null)
		{
			Console.WriteLine("Start Download/Начало загрузки...");
			filePath ??= saveFilePath;
			IFormatter formatter = new BinaryFormatter();
			using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				GameState state = (GameState)formatter.Deserialize(stream);
				dungeonMap = state.DungeonMap;
				playerX = state.PlayerX;
				playerY = state.PlayerY;
				treasureCount = state.TreasureCount;
				moveCount = state.MoveCount;
				enemies = state.Enemies;
				mapSize = state.MapSize;
				totalTreasures = state.TotalTreasures;
				enemyCount = state.EnemyCount;
			}

			isGameInitialized = true;
			Console.WriteLine("Game was download/Игра загружена.");
			if (!isTestMode) Console.ReadKey();
		}


		static void ResetGameState()
		{
			dungeonMap = null;
			treasureCount = 0;
			moveCount = 0;
			enemies = null;
			isGameInitialized = false;
		}
	}
}



