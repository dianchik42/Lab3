using Lab3;
namespace TestLab3
{
	public class UnitTest1
	{
		[Fact]
		public void InitializeGame_ShouldCreateValidDungeonMap()
		{
			// Arrange
			Program.ResetState();

			// Act
			Program.InitializeGame();

			// Assert
			Assert.NotNull(Program.GetDungeonMap());
			Assert.Equal(Program.GetMapSize(), Program.GetDungeonMap().GetLength(0)); // Проверка размера карты
		}

		[Fact]
		public void SaveGame_ShouldCreateValidSaveFile()
		{
			// Arrange
			Program.isTestMode = true;
			Program.ResetState();
			Program.InitializeGame();
			string tempFile = Path.GetTempFileName();

			try
			{
				Program.saveFilePath = tempFile;

				// Act
				Program.SaveGame();

				// Assert
				Assert.True(File.Exists(tempFile), "Файл сохранения не был создан.");
			}
			finally
			{
				// Cleanup
				File.Delete(tempFile);
			}
		}


		[Fact]
		public void LoadGame_ShouldRestoreCorrectState()
		{
			// Arrange
			Program.isTestMode = true;
			Program.ResetState();
			Program.InitializeGame();
			string tempFile = Path.GetTempFileName();

			try
			{
				Program.saveFilePath = tempFile;
				Program.SaveGame();

				// Reset state
				Program.ResetState();

				// Act
				Program.LoadGame();

				// Assert
				Assert.NotNull(Program.GetDungeonMap());
				Assert.Equal(Program.GetMapSize(), Program.GetDungeonMap().GetLength(0)); // Проверка размера карты
			}
			finally
			{
				// Cleanup
				File.Delete(tempFile);
			}
		}

		[Fact]
		public void ResetState_ShouldClearGameState()
		{
			// Arrange

			Program.InitializeGame();

			// Act
			Program.ResetState();

			// Assert
			Assert.Null(Program.dungeonMap);
			Assert.Equal(0, Program.treasureCount);
			Assert.Equal(0, Program.moveCount);
			Assert.Null(Program.enemies);
			Assert.False(Program.isGameInitialized);
		}


	}
}