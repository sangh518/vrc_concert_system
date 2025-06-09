
using System.Text;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum ChatMessageType
{
    Unknown = 0,
    Chat = 1,
    Sub = 2,
    Donate = 3,
    Ad = 4
}

public class MidiListenTest : UdonSharpBehaviour
{

  // 상수 정의
    private const char SPLIT_CHAR = '\x1F'; // Unit Separator (US)

    // 메시지 타입 정의


    // 비트 버퍼 (최대 비트 수를 미리 정의)
    private bool[] bitBuffer = new bool[8192]; // 예: 최대 8192 비트
    private int bitBufferIndex = 0;

    // 현재 메시지 상태
    private ChatMessageType currentMessageType = ChatMessageType.Unknown;
    private int expectedBits = 0;
    private int receivedBits = 0;

    void Start()
    {
        // MIDI 입력을 초기화하고 이벤트에 구독합니다.
        // 사용하는 MIDI 라이브러리에 따라 다릅니다.
        // 예를 들어:
        // MidiInput.OnNoteOn += MidiNoteOn;
        // MidiInput.OnNoteOff += MidiNoteOff;
    }

    // MIDI Note On 이벤트 핸들러
    public override void MidiNoteOn(int channel, int number, int velocity)
    {
        // 18비트 값 재구성
        int val = (channel << 14) | (number << 7) | velocity;

        // 18비트를 개별 비트로 분리하여 버퍼에 저장
        for (int i = 17; i >= 0; i--)
        {
            bool bit = ((val >> i) & 1) == 1;
            if (bitBufferIndex < bitBuffer.Length)
            {
                bitBuffer[bitBufferIndex++] = bit;
                receivedBits++;
            }
            else
            {
                Debug.LogWarning("비트 버퍼가 가득 찼습니다.");
                break;
            }
        }

        // 필요한 경우 추가 로직을 여기에 구현할 수 있습니다.
    }

    // MIDI Note Off 이벤트 핸들러
    public override void MidiNoteOff(int channel, int number, int velocity)
    {
        // 18비트 값 재구성
        int val = (channel << 14) | (number << 7) | velocity;

        // 메시지 타입, 전송 시작 여부, 길이 추출
        int messageType = (val >> 14) & 0xF;
        int isStart = (val >> 13) & 0x1;
        int length = val & 0x1FFF; // 13비트

        if (isStart == 1)
        {
            // 새로운 메시지의 시작
            currentMessageType = (ChatMessageType)messageType;
            expectedBits = length;
            bitBufferIndex = 0;
            receivedBits = 0;
            //Debug.Log($"새로운 {currentMessageType} 메시지 시작. 예상 비트 수: {expectedBits}");
        }
        
        else
        {
            // 메시지의 끝
            //Debug.Log($"메시지 종료: {currentMessageType}. 받은 비트 수: {receivedBits}");
            ProcessCurrentMessage();
            currentMessageType = ChatMessageType.Unknown;
            expectedBits = 0;
            bitBufferIndex = 0;
            receivedBits = 0;
        }
    }

    // 현재 메시지 처리
    private void ProcessCurrentMessage()
    {
        if (receivedBits < expectedBits)
        {
            Debug.LogWarning("받은 비트 수가 예상보다 적습니다.");
            return;
        }

        // 비트를 바이트 배열로 변환
        byte[] bytes = BitsToBytes(bitBuffer, expectedBits);

        // 바이트를 문자열로 변환
        string message = Encoding.UTF8.GetString(bytes).TrimEnd('\0');

        // SPLIT_CHAR로 메시지 분리
        int splitIndex = message.IndexOf(SPLIT_CHAR);
        if (splitIndex == -1)
        {
            Debug.LogWarning("유효하지 않은 메시지 형식입니다.");
            return;
        }

        string name = message.Substring(0, splitIndex);
        string content = message.Substring(splitIndex + 1);

        // 메시지 타입에 따라 처리
        switch (currentMessageType)
        {
            case ChatMessageType.Chat:
                HandleChat(name, content);
                break;
            case ChatMessageType.Sub:
                HandleSub(name, content);
                break;
            case ChatMessageType.Donate:
                HandleDonate(name, content);
                break;
            case ChatMessageType.Ad:
                HandleAd(name, content);
                break;
            default:
                Debug.LogWarning("알 수 없는 메시지 타입입니다.");
                break;
        }
    }

    // 비트 배열을 바이트 배열로 변환
    private byte[] BitsToBytes(bool[] bits, int length)
    {
        int byteLength = (length + 7) / 8;
        byte[] bytes = new byte[byteLength];
        for (int i = 0; i < length; i++)
        {
            int byteIndex = i / 8;
            int bitIndex = 7 - (i % 8);
            if (bits[i])
            {
                bytes[byteIndex] |= (byte)(1 << bitIndex);
            }
        }
        return bytes;
    }

    // 메시지 핸들러들
    private void HandleChat(string name, string content)
    {
        Debug.Log($"채팅 - {name}: {content}");
        // 채팅 처리 로직 구현
    }

    private void HandleSub(string name, string countStr)
    {
        if (int.TryParse(countStr, out int count))
        {
            Debug.Log($"구독 - {name}: {count}");
            // 구독 처리 로직 구현
        }
        else
        {
            Debug.LogWarning("유효하지 않은 구독 수입니다.");
        }
    }

    private void HandleDonate(string name, string countStr)
    {
        if (int.TryParse(countStr, out int count))
        {
            Debug.Log($"도네이트 - {name}: {count}");
            // 도네이트 처리 로직 구현
        }
        else
        {
            Debug.LogWarning("유효하지 않은 도네이트 수입니다.");
        }
    }

    private void HandleAd(string name, string countStr)
    {
        if (int.TryParse(countStr, out int count))
        {
            Debug.Log($"광고 - {name}: {count}");
            // 광고 처리 로직 구현
        }
        else
        {
            Debug.LogWarning("유효하지 않은 광고 수입니다.");
        }
    }
}
