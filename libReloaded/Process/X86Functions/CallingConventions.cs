﻿/*
    [Reloaded] Mod Loader Common Library (libReloaded)
    The main library acting as common, shared code between the Reloaded Mod 
    Loader Launcher, Mods as well as plugins.
    Copyright (C) 2018  Sewer. Sz (Sewer56)

    [Reloaded] is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    [Reloaded] is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>
*/

namespace Reloaded.Process.X86Functions
{
    /// <summary>
    /// This class provides information on various commonly seen calling conventions and information on how
    /// to call functions utilising these calling conventions from Reloaded code.
    /// </summary>
    public enum CallingConventions
    {
        /// <summary>
        /// Attribute [UnmanagedFunctionPointer(CallingConvention.Cdecl)] on Delegate
        /// Usage: Native C# GetDelegateForFunctionPointer() for calling, 
        ///        append complete ReloadedFunction attribute for function hooking.
        /// 
        /// Parameters are passed right to left onto the function pushing onto the stack.
        /// Calling function pops its own arguments from the stack. 
        /// (The calling function must manually restore the stack to previous state)
        /// 
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    N/A
        ///     ReturnRegister:     EAX    
        ///     Cleanup:            Caller
        /// </summary>
        Cdecl,

        /// <summary>
        /// Attribute [UnmanagedFunctionPointer(CallingConvention.Stdcall)] on Delegate
        /// Usage: Native C# GetDelegateForFunctionPointer() for calling, 
        ///        append complete ReloadedFunction attribute for function hooking.
        /// 
        /// Parameters are passed right to left onto the function pushing onto the stack.
        /// Called function pops its own arguments from the stack.
        /// 
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    N/A
        ///     ReturnRegister:     EAX    
        ///     Cleanup:            Callee
        /// </summary>
        Stdcall,

        /// <summary>
        /// Attribute [UnmanagedFunctionPointer(CallingConvention.Cdecl)] on Delegate
        /// Usage: FunctionWrapper class.
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    ECX, EDX
        ///     ReturnRegister:     EAX    
        ///     Cleanup:            Caller
        /// 
        /// The first two arguments are passed in from left to right into ECX and EDX.
        /// The others are passed in right to left onto stack.
        /// Caller cleanup: If necessary, the stack is cleaned up by the caller.
        /// </summary>
        Fastcall,

        /// <summary>
        /// Attribute [UnmanagedFunctionPointer(CallingConvention.Cdecl)] on Delegate
        /// Usage (GCC Thiscall):       Native C# GetDelegateForFunctionPointer(), 1st parameter for `this` object pointer.
        /// Usage (Microsoft Thiscall): FunctionWrapper class.
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    ECX
        ///     ReturnRegister:     EAX
        ///     Cleanup:            Callee
        /// 
        /// Variant of Stdcall where the pointer of the `this` object is passed into ECX and
        /// rest of the parameters passed as usual. The Callee cleans the stack.
        /// You should define your delegates with the (this) object pointer (IntPtr) as first parameter from the left.
        /// 
        /// GCC thiscall: 
        ///     `this` object passed as first parameter, caller stack cleanup, use cdecl with `this` 
        ///     object pointer (IntPtr) as first parameter instead.
        /// </summary>
        Thiscall,

        /// <summary>
        /// A name given to custom calling conventions by Hex-Rays (IDA) that are cleaned up by the caller.
        /// 
        /// Attribute [UnmanagedFunctionPointer(CallingConvention.Cdecl)] on Delegate
        /// Usage: FunctionWrapper class.
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    Depends on Function
        ///     ReturnRegister:     Depends on Function
        ///     Cleanup:            Caller
        /// </summary>
        Usercall,

        /// <summary>
        /// A name given to custom calling conventions by Hex-Rays (IDA) that are cleaned up by the callee.
        /// 
        /// Attribute [UnmanagedFunctionPointer(CallingConvention.Cdecl)] on Delegate
        /// Usage: FunctionWrapper class.
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    Depends on Function
        ///     ReturnRegister:     Depends on Function
        ///     Cleanup:            Callee
        /// </summary>
        Userpurge
    }
}
