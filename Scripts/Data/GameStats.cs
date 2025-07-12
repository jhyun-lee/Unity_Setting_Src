using System;
using System.Collections.Generic;

[Serializable]
public class GameStats
{
    // 총 플레이 시간 (초 단위)
    public float totalPlayTime = 0f;
    
    // 게임 플레이 횟수
    public int totalGamesPlayed = 0;
    
    // 총 점수
    public long totalScore = 0;
    
    // 업적 배열 (업적 ID들의 리스트)
    public List<string> achievements = new List<string>();
    
    // 최고 점수
    public long highScore = 0;
    
    // 평균 점수
    public float averageScore = 0f;
    
    // 마지막 게임 점수
    public long lastGameScore = 0;
    
    // 연속 플레이 횟수
    public int consecutiveGames = 0;
    
    // 최대 연속 플레이 횟수
    public int maxConsecutiveGames = 0;
    
    // 평균 플레이 시간 (초)
    public float averagePlayTime = 0f;
    
    // 총 클리어한 레벨 수
    public int totalLevelsCleared = 0;
    
    // 최고 클리어한 레벨
    public int highestLevelCleared = 0;
    
    // 생성자
    public GameStats()
    {
        // 기본값들은 이미 위에서 설정됨
    }
    
    // 평균 점수 계산
    public void CalculateAverageScore()
    {
        if (totalGamesPlayed > 0)
        {
            averageScore = (float)totalScore / totalGamesPlayed;
        }
    }
    
    // 평균 플레이 시간 계산
    public void CalculateAveragePlayTime()
    {
        if (totalGamesPlayed > 0)
        {
            averagePlayTime = totalPlayTime / totalGamesPlayed;
        }
    }
    
    // 새로운 게임 결과 추가
    public void AddGameResult(long score, float playTime, int levelCleared = 0)
    {
        totalGamesPlayed++;
        totalScore += score;
        totalPlayTime += playTime;
        lastGameScore = score;
        
        if (score > highScore)
        {
            highScore = score;
        }
        
        if (levelCleared > 0)
        {
            totalLevelsCleared++;
            if (levelCleared > highestLevelCleared)
            {
                highestLevelCleared = levelCleared;
            }
        }
        
        // 평균값들 재계산
        CalculateAverageScore();
        CalculateAveragePlayTime();
    }
    
    // 업적 추가
    public void AddAchievement(string achievementId)
    {
        if (!achievements.Contains(achievementId))
        {
            achievements.Add(achievementId);
        }
    }
    
    // 업적 확인
    public bool HasAchievement(string achievementId)
    {
        return achievements.Contains(achievementId);
    }
    
    // 통계 초기화
    public void ResetStats()
    {
        totalPlayTime = 0f;
        totalGamesPlayed = 0;
        totalScore = 0;
        achievements.Clear();
        highScore = 0;
        averageScore = 0f;
        lastGameScore = 0;
        consecutiveGames = 0;
        maxConsecutiveGames = 0;
        averagePlayTime = 0f;
        totalLevelsCleared = 0;
        highestLevelCleared = 0;
    }
} 