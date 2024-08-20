namespace EldenBingo.GameInterop;

public class EventManager {
    private GameProcessHandler _gameHandler;
    // https://defuse.ca/online-x86-assembler.htm
    public static readonly byte[] Asm = {
        0x48, 0xb9, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0x0f,   // movabs rcx,0xfffffff00000000       ;EventFlagMan
        0x48, 0xb8, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0x0f,   // movabs rax,0xfffffff00000000       ;SetEventFlagFunction
        0x41, 0xb0, 0x01,                                             // mov    r8b,0x1                     ;State - True
        0x48, 0x83, 0xec, 0x08,                                       // sub    rsp,0x8                     ;Flag needs to be written to memeory
        0xc7, 0x04, 0x24, 0xba, 0xaa, 0x12, 0x01,                     // mov    DWORD PTR [rsp],0x112aaba   ;Flag - 18000570
        0x48, 0x8d, 0x14, 0x24,                                       // lea    rdx,[rsp]                   ;Flag Pointer
        0x48, 0x83, 0xec, 0x28,                                       // sub    rsp,0x28
        0xff, 0xd0,                                                   // call   rax
        0x48, 0x83, 0xc4, 0x30,                                       // add    rsp,0x30
        0xc3,                                                         // ret 
    };

    public EventManager(GameProcessHandler processHandler) {
        _gameHandler = processHandler;
    }

    public const int EventFlagManOffset = 0x2;
    public const int SetEventFlagFunctionOffset = 0xC;
    // Incase we wanna change it in the future. For now, it's hardcoded. 
    public const int StateOffset = 0x16;
    public const int EventFlagIdOffset = 0x1E;
    
    // Partially setup for later, in case we want to use this, later, for something else.
    public void SetEventFlag(uint eventId, bool state) {
        byte[] asm = Asm.ToArray();
        IntPtr eventManPtr = _gameHandler.GetEventManPtr();
        Array.Copy(BitConverter.GetBytes(eventManPtr.ToInt64()), 0, asm, EventFlagManOffset, sizeof(long));
        IntPtr setEventPtr = _gameHandler.GetSetEventFlagPtr();
        Array.Copy(BitConverter.GetBytes(setEventPtr.ToInt64()), 0, asm, SetEventFlagFunctionOffset, sizeof(long));
        _gameHandler.ExecuteAsm(asm);
    }
    
}