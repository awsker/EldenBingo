namespace EldenBingo.GameInterop;

public static class MachineCode {
    public const int EventFlagManOffset = 0x2;
    public const int EventFlagFunctionOffset = 0xC;
    public const int EventFlagIdOffset = 0x1B;
    public const int SetEventFlagStateOffset = 0x25;

    // https://defuse.ca/online-x86-assembler.htm
    public static readonly byte[] SetEventFlagMachineCode = {
        0x48, 0xb9, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff,   // movabs rcx,0xffffffff00000000      ;EventFlagMan
        0x48, 0xb8, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff,   // movabs rax,0xffffffff00000000      ;SetEventFlagFunction
        0x48, 0x83, 0xec, 0x08,                                       // sub    rsp,0x8                     ;Flag needs to be written to memory
        0xc7, 0x04, 0x24, 0x00, 0x00, 0x00, 0xf0,                     // mov    DWORD PTR [rsp],0xF0000000  ;Flag
        0x48, 0x8d, 0x14, 0x24,                                       // lea    rdx,[rsp]                   ;Flag Pointer
        0x41, 0xb0, 0x01,                                             // mov    r8b,0x1                     ;State
        0x48, 0x83, 0xec, 0x28,                                       // sub    rsp,0x28
        0xff, 0xd0,                                                   // call   rax
        0x48, 0x83, 0xc4, 0x30,                                       // add    rsp,0x30
        0xc3,                                                         // ret
    };
    
    public const int IsEventFlagReturnPointerOffset = 0x2B; 
    public static readonly byte[] IsEventFlagMachineCode = {
        0x48, 0xb9, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, // movabs rcx,0xffffffff00000000	;EventFlagMan  
        0x48, 0xb8, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, // movabs rax,0xffffffff00000000 	;IsEventFlagFunction 
        0x48, 0x83, 0xec, 0x08,                                     // sub    rsp,0x8                   ;Flag needs to be written to memory 
        0xc7, 0x04, 0x24, 0x00, 0x00, 0x00, 0xf0,                   // mov    DWORD PTR [rsp],0xF0000000;Flag 
        0x48, 0x8d, 0x14, 0x24,                                     // lea    rdx,[rsp]                 ;Flag Pointer 
        0x48, 0x83, 0xec, 0x28,      								// sub    rsp,0x28 
        0xff, 0xd0,                   								// call   rax 
        0x48, 0xb9, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff,	// movabs rcx,0xffffffff00000000	;return value pointer 
        0x48, 0x89, 0x01,                							// mov    QWORD PTR [rcx],rax 
        0x48, 0x83, 0xc4, 0x30,             						// add    rsp,0x30 
        0xc3,                                                       // ret  
        
    };
    
    public static byte[] SetEventFlag(uint eventId, bool state, long eventManPtr, long setEventPtr) {
        var machineCode = SetEventFlagMachineCode.ToArray();
        // Set EventFlagMan pointer
        Array.Copy(BitConverter.GetBytes(eventManPtr), 0, machineCode, EventFlagManOffset, sizeof(long));
        // Set Function Call Address
        Array.Copy(BitConverter.GetBytes(setEventPtr), 0, machineCode, EventFlagFunctionOffset, sizeof(long));
        // Set Event Id
        Array.Copy(BitConverter.GetBytes(eventId), 0, machineCode, EventFlagIdOffset, sizeof(uint));
        // Set State
        Array.Copy(BitConverter.GetBytes(state), 0, machineCode, SetEventFlagStateOffset, sizeof(bool));
        return machineCode;
    }
    
    public static byte[] IsEventFlag(uint eventId, long eventManPtr, long isEventPtr, IntPtr returnPtr) {
        var machineCode = IsEventFlagMachineCode.ToArray();
        // Set EventFlagMan pointer
        Array.Copy(BitConverter.GetBytes(eventManPtr), 0, machineCode, EventFlagManOffset, sizeof(long));
        // Is Function Call Address
        Array.Copy(BitConverter.GetBytes(isEventPtr), 0, machineCode, EventFlagFunctionOffset, sizeof(long));
        // Set Event Id
        Array.Copy(BitConverter.GetBytes(eventId), 0, machineCode, EventFlagIdOffset, sizeof(uint));
        // Return Pointer
        Array.Copy(BitConverter.GetBytes(returnPtr.ToInt64()), 0, machineCode, IsEventFlagReturnPointerOffset, sizeof(ulong));
        return machineCode;
    }


}