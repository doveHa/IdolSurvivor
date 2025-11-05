using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum DiceCheckResult
{
    CriticalSuccess,
    Success,
    Normal,
    Failure
}

namespace Script
{
    public class DiceRoller : MonoBehaviour
    {
        public static int lastRollResult = 0;

        [Header("UI Component")] public Image diceImage;

        [Header("Dice Sprites")] public Sprite[] diceSprites;

        [Header("Animation Settings")] public int diceSides = 6;
        public float rollDuration = 0.5f;
        public float changeInterval = 0.05f;

        public static void RollDice(int diceSides)
        {
            int result = Random.Range(1, diceSides + 1);

            lastRollResult = result;

            Debug.Log($"D{diceSides} �ֻ��� ���: {result}");
        }

        public void RollDiceWithAnimation()
        {
            StopAllCoroutines();
            StartCoroutine(AnimateDiceRoll());
        }

        IEnumerator AnimateDiceRoll()
        {
            if (diceSprites == null || diceSprites.Length != 6 || diceImage == null)
            {
                Debug.LogError("�ֻ��� ��������Ʈ �迭�� 6������ �ϸ�, Image ������Ʈ�� �Ҵ�Ǿ�� �մϴ�.");
                yield break;
            }

            float startTime = Time.time;

            while (Time.time < startTime + rollDuration)
            {
                int randomSpriteIndex = Random.Range(0, diceSprites.Length);
                diceImage.sprite = diceSprites[randomSpriteIndex];

                yield return new WaitForSeconds(changeInterval);
            }

            int result = Random.Range(1, diceSides + 1);
            lastRollResult = result;

            int finalSpriteIndex = result - 1;

            diceImage.sprite = diceSprites[finalSpriteIndex];

            Debug.Log($"D{diceSides} �ֻ��� ���� ���: {result}");
        }

        public static DiceCheckResult JudgeRollResult(int roll)
        {
            if (roll == 6)
            {
                return DiceCheckResult.CriticalSuccess; // 대성공
            }
            else if (roll == 4 || roll == 5)
            {
                return DiceCheckResult.Success; // 성공
            }
            else // roll == 1 || roll == 2
            {
                return DiceCheckResult.Failure; // 실패
            }
        }

        public static DiceCheckResult JudgeRollResultWithNormal(int roll)
        {
            if (roll == 6)
            {
                return DiceCheckResult.CriticalSuccess; // 대성공
            }
            else if (roll == 4 || roll == 5)
            {
                return DiceCheckResult.Success; // 성공
            }
            else if (roll == 3)
            {
                return DiceCheckResult.Normal; // 보통
            }
            else // roll == 1 || roll == 2
            {
                return DiceCheckResult.Failure; // 실패
            }
        }
    }
}