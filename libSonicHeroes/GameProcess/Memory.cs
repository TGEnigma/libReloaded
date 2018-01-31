﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SonicHeroes.GameProcess.Native;

namespace SonicHeroes.GameProcess
{
    /// <summary>
    /// Class which allows the manipulation of in-game memory status.
    /// </summary>
    public static class Memory
    {
        /// <summary>
        /// Allows for allocation of memory space inside the target process. 
        /// The return value for this method is the address at which the new memory has been reserved. 
        /// </summary>
        /// <param name="process">The process object of the game, Process.GetCurrentProcess() if injected into the game.</param>
        /// <param name="length">Length of free bytes you want to allocate.</param>
        /// <returns>Base pointer address to the newly allocated memory.</returns>
        public static IntPtr AllocateMemory(this Process process, int length)
        {
            // Call VirtualAllocEx to allocate memory of fixed chosen size.
            return VirtualAllocEx(process.Handle, IntPtr.Zero, (IntPtr)length,
                AllocationType.Commit | AllocationType.Reserve,
                MemoryProtection.ExecuteReadWrite);
        }

        /// <summary>
        /// Allows for the freeing of memory space inside the target process. 
        /// Releases memory such that it may be cleaned and re-used by the Windows Operating System.
        /// </summary>
        /// <param name="process">The process object of the game, Process.GetCurrentProcess() if injected into the game.</param>
        /// <param name="address">The address of the first byte you want to free memory from.</param>
        /// <returns>A value that is not 0 if the operation is successful.</returns>
        public static bool FreeMemory(this Process process, IntPtr address)
        {
            return VirtualFreeEx(process.Handle, address, 0,
                FreeType.Decommit | FreeType.Release);
        }

        /// <summary>
        /// Reads a specified specific amount of bytes to the process using the native ReadProcessMemory call. 
        /// </summary>
        /// <param name="process">The process object of the game, Process.GetCurrentProcess() if injected into the game.</param>
        /// <param name="address">The address of the first byte you want to write memory to.</param>
        /// <returns>The bytes which have been read from the memory at the specified offset and length.</returns>
        public static T ReadMemory<T>(this Process process, IntPtr address)
        {
            // Retrieve the type of the passed in Generic.
            Type type = typeof(T);

            // Retrieve the size of T.
            int size = Marshal.SizeOf(type);

            // Initializes the buffer of the length of the data to be read.
            byte[] buffer = new byte[size];
            
            // Read from the game memory.
            Marshal.Copy(address, buffer, 0, size);

            // Return the read memory.
            return (T)Convert.ChangeType(buffer, typeof(T));
        }

        /// <summary>
        /// See <see cref="ReadMemory"/>. Use only if you are denied normally reading memory. Reads game memory but first removes protection on a page of memory before reading the memory followed by restoring it.
        /// </summary>
        /// <param name="process">The process object of the game, Process.GetCurrentProcess() if injected into the game.</param>
        /// <param name="address">The address of the first byte you want to write memory to.</param>
        /// <returns>The bytes which have been read from the memory at the specified offset and length.</returns>
        public static T ReadMemorySafe<T>(this Process process, IntPtr address)
        {
            // Retrieve the type of the passed in Generic.
            Type type = typeof(T);

            // Retrieve the size of T.
            int size = Marshal.SizeOf(type);

            // Initializes the buffer of the length of the data to be read.
            byte[] buffer = new byte[size];

            // Store the old memory protection flags.
            MemoryProtection OldProtection;

            // Mark memory we are reading to as ExecuteReadWrite
            VirtualProtect(address, (uint)size, MemoryProtection.ExecuteReadWrite, out OldProtection);

            // Read from the game memory.
            Marshal.Copy(address, buffer, 0, size);

            // Restore the old memory protection.
            VirtualProtect(address, (uint)size, OldProtection, out OldProtection);

            // Return the read memory.
            return (T)Convert.ChangeType(buffer, typeof(T));
        }

        /// <summary>
        /// Reads a specified specific amount of bytes using ReadProcessMemory(), does no conversion and returns as untouched byte array. 
        /// </summary>
        /// <param name="process">The process object of the game, Process.GetCurrentProcess() if injected into the game.</param>
        /// <param name="address">The address of the first byte you want to write memory to.</param>
        /// <param name="length">The value you want to write at the address as a byte array.</param>
        /// <returns>The bytes which have been read from the memory at the specified offset and length.</returns>
        public static byte[] ReadMemory(this Process process, IntPtr address, int length)
        {
            // Initialize the buffer of required length.
            byte[] buffer = new byte[length];

            // Read from the game memory.
            Marshal.Copy(address, buffer, 0, length);

            // Return the buffer.
            return buffer;
        }

        /// <summary>
        /// <see cref="ReadMemory"/> Use only if you are denied normally reading memory. Reads game memory but first removes protection on a page of memory before reading the memory followed by restoring it. 
        /// </summary>
        /// <param name="process">The process object of the game, Process.GetCurrentProcess() if injected into the game.</param>
        /// <param name="address">The address of the first byte you want to write memory to.</param>
        /// <param name="length">The value you want to write at the address as a byte array.</param>
        /// <returns>The bytes which have been read from the memory at the specified offset and length.</returns>
        public static byte[] ReadMemorySafe(this Process process, IntPtr address, int length)
        {
            // Initialize the buffer of required length.
            byte[] buffer = new byte[length];

            // Store the old memory protection flags.
            MemoryProtection OldProtection;

            // Mark memory we are reading to as ExecuteReadWrite
            VirtualProtect(address, (uint)length, MemoryProtection.ExecuteReadWrite, out OldProtection);

            // Read from the game memory.
            Marshal.Copy(address, buffer, 0, length);

            // Restore the old memory protection.
            VirtualProtect(address, (uint)length, OldProtection, out OldProtection);

            // Return the buffer.
            return buffer;
        }

        /// <summary>
        /// Writes a specified specific amount of bytes to the process using the native WriteProcessMemory call. 
        /// </summary>
        /// <param name="process">The process object of the game, Process.GetCurrentProcess() if injected into the game.</param>
        /// <param name="address">The address of the first byte you want to write memory to.</param>
        /// <param name="data">The value you want to write at the address as a byte array.</param>
        /// <returns>Whether the write operation has been successful as true/false</returns>
        public static void WriteMemory(this Process process, IntPtr address, byte[] data)
        {
            // Write the process memory.            
            Marshal.Copy(data, 0, address, data.Length);
        }

        /// <summary>
        /// See <see cref="WriteMemory"/> Use only if you are denied normally writing memory. Writes game memory but first removes protection on a page of memory before writing the memory followed by restoring it. 
        /// </summary>
        /// <param name="process">The process object of the game, Process.GetCurrentProcess() if injected into the game.</param>
        /// <param name="address">The address of the first byte you want to write memory to.</param>
        /// <param name="data">The value you want to write at the address as a byte array.</param>
        /// <returns>Whether the write operation has been successful as true/false</returns>
        public static void WriteMemorySafe(this Process process, IntPtr address, byte[] data)
        {
            // Store the old memory protection flags.
            MemoryProtection OldProtection;

            // Mark memory we are writing to as ExecuteReadWrite
            VirtualProtect(address, (uint)data.Length, MemoryProtection.ExecuteReadWrite, out OldProtection);

            // Write the process memory.            
            Marshal.Copy(data, 0, address, data.Length);

            // Restore the old memory protection.
            VirtualProtect(address, (uint)data.Length, OldProtection, out OldProtection);
        }

        /// <summary>
        /// Reads a specified specific amount of bytes to the process using the native ReadProcessMemory call. 
        /// </summary>
        /// <param name="process">The process object of the game, Process.GetCurrentProcess() if injected into the game.</param>
        /// <param name="address">The address of the first byte you want to write memory to.</param>
        /// <returns>The bytes which have been read from the memory at the specified offset and length.</returns>
        public static T ReadMemoryExternal<T>(this Process process, IntPtr address)
        {
            // Retrieve the type of the passed in Generic.
            Type type = typeof(T);
            
            // Retrieve the size of T.
            int size = Marshal.SizeOf(type);

            // Initializes the buffer of the length of the data to be read.
            byte[] buffer = new byte[size];

            // Store the amount of bytes read.
            IntPtr bytesRead;

            // Read from the game memory.
            ReadProcessMemory(process.Handle, address, buffer, size, out bytesRead);
            
            // Return the read memory.
            return (T)Convert.ChangeType(buffer, typeof(T));
        }

        /// <summary>
        /// See <see cref="ReadMemoryExternal"/>. Use only if you are denied normally reading memory. Reads game memory but first removes protection on a page of memory before reading the memory followed by restoring it.  
        /// </summary>
        /// <param name="process">The process object of the game, Process.GetCurrentProcess() if injected into the game.</param>
        /// <param name="address">The address of the first byte you want to write memory to.</param>
        /// <returns>The bytes which have been read from the memory at the specified offset and length.</returns>
        public static T ReadMemoryExternalSafe<T>(this Process process, IntPtr address)
        {
            // Retrieve the type of the passed in Generic.
            Type type = typeof(T);

            // Retrieve the size of T.
            int size = Marshal.SizeOf(type);

            // Initializes the buffer of the length of the data to be read.
            byte[] buffer = new byte[size];

            // Store the amount of bytes read.
            IntPtr bytesRead;

            // Store the old memory protection flags.
            MemoryProtection OldProtection;

            // Mark memory we are writing to as ExecuteReadWrite
            VirtualProtect(address, (uint)size, MemoryProtection.ExecuteReadWrite, out OldProtection);

            // Read from the game memory.
            ReadProcessMemory(process.Handle, address, buffer, size, out bytesRead);

            // Restore the old memory protection.
            VirtualProtect(address, (uint)size, OldProtection, out OldProtection);

            // Return the read memory.
            return (T)Convert.ChangeType(buffer, typeof(T));
        }

        /// <summary>
        /// Reads a specified specific amount of bytes using ReadProcessMemory(), does no conversion and returns as untouched byte array. 
        /// </summary>
        /// <param name="process">The process object of the game, Process.GetCurrentProcess() if injected into the game.</param>
        /// <param name="address">The address of the first byte you want to write memory to.</param>
        /// <param name="length">The value you want to write at the address as a byte array.</param>
        /// <returns>The bytes which have been read from the memory at the specified offset and length.</returns>
        public static byte[] ReadMemoryExternal(this Process process, IntPtr address, int length)
        {
            // Initialize the buffer of required length.
            byte[] buffer = new byte[length];

            // Store the amount of bytes read.
            IntPtr bytesRead;

            // Read from the game memory.
            ReadProcessMemory(process.Handle, address, buffer, length, out bytesRead);

            // Return the buffer.
            return buffer;
        }

        /// <summary>
        /// Writes a specified specific amount of bytes to the process using the native WriteProcessMemory call. 
        /// </summary>
        /// <param name="process">The process object of the game, Process.GetCurrentProcess() if injected into the game.</param>
        /// <param name="address">The address of the first byte you want to write memory to.</param>
        /// <param name="data">The value you want to write at the address as a byte array.</param>
        /// <returns>Whether the write operation has been successful as true/false</returns>
        public static bool WriteMemoryExternal(this Process process, IntPtr address, byte[] data)
        {
            // Store the amount of bytes written.
            IntPtr bytesWrite;
            
            // Store the old memory protection flags.
            MemoryProtection OldProtection;

            // Mark memory we are writing to as ExecuteReadWrite
            VirtualProtect(address, (uint)data.Length, MemoryProtection.ExecuteReadWrite, out OldProtection);

            // Write the process memory.
            bool success = WriteProcessMemory(process.Handle, address, data, data.Length, out bytesWrite);

            // Restore the old memory protection.
            VirtualProtect(address, (uint)data.Length, OldProtection, out OldProtection);

            // Return value
            return success;
        }

        /// <summary>
        /// See <see cref="WriteMemoryExternal"/>  Use only if you are denied normally writing memory. Writes game memory but first removes protection on a page of memory before writing the memory followed by restoring it.
        /// </summary>
        /// <param name="process">The process object of the game, Process.GetCurrentProcess() if injected into the game.</param>
        /// <param name="address">The address of the first byte you want to write memory to.</param>
        /// <param name="data">The value you want to write at the address as a byte array.</param>
        /// <returns>Whether the write operation has been successful as true/false</returns>
        public static bool WriteMemoryExternalSafe(this Process process, IntPtr address, byte[] data)
        {
            // Store the amount of bytes written.
            IntPtr bytesWrite;

            // Store the old memory protection flags.
            MemoryProtection OldProtection;

            // Mark memory we are writing to as ExecuteReadWrite
            VirtualProtect(address, (uint)data.Length, MemoryProtection.ExecuteReadWrite, out OldProtection);

            // Write the process memory.
            bool success = WriteProcessMemory(process.Handle, address, data, data.Length, out bytesWrite);

            // Restore the old memory protection.
            VirtualProtect(address, (uint)data.Length, OldProtection, out OldProtection);

            // Return value
            return success;
        }

        /// <summary>
        /// Generally useful when looking for game code 
        /// for games which receive periodic updates.
        /// Attempts to locate the given pattern inside a supplied memory region,
        /// with support for checking against a given mask. 
        /// If the pattern is found, the offset within the supplied memory region
        /// where the pattern matches is returned.
        /// </summary>
        /// <param name="memoryRegion">The memory region in which to look for the specified pattern.</param>
        /// <param name="bytePattern">Byte pattern to look for in the dumped region. e.g. new byte[] { 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 }</param>
        /// <param name="stringMask">The mask string to compare against. `x` represents check while `?` ignores. Each `x` and `?` represent 1 byte.</param>
        /// <returns>0 if not found, offset in memory region if found.</returns>
        public static IntPtr FindPattern(byte[] memoryRegion, byte[] bytePattern, string stringMask)
        {
            try
            {
                // Ensure mask and pattern length match.
                if (stringMask.Length != bytePattern.Length) { return IntPtr.Zero; }

                // Loop the region and look for the pattern.
                for (int x = 0; x < memoryRegion.Length; x++)
                {
                    // Check for the mask, incrementing start offset each time.
                    if (MaskCheck(memoryRegion, bytePattern, stringMask, x))
                    {
                        // The pattern was found, return the offset of it.
                        return new IntPtr(x);
                    }
                }

                // Pattern was not found.
                return IntPtr.Zero;
            }
            catch (Exception ex) { return IntPtr.Zero; }
        }


        /// <summary>
        /// Compares the current pattern byte to the supplied memory dump
        /// byte to check for a match. Uses wildcards to skip bytes that
        /// are deemed unneeded in the compares.
        /// </summary>
        /// <param name="memoryRegion">The memory region in which to look for the specified pattern.</param>
        /// <param name="bytePattern">Byte pattern to look for in the dumped region.</param>
        /// <param name="stringMask">The mask string to compare against. `XX` represents check while `??` ignores.</param>
        /// <param name="startOffset">Offset in the dump to start comparing bytes against at.</param>
        /// <returns>Boolean depending on if the pattern was found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Compile inline.
        private static bool MaskCheck(byte[] memoryRegion, byte[] bytePattern, string stringMask, int startOffset)
        {
            // Loop the pattern and compare to the mask and our memory region.
            for (int x = 0; x < bytePattern.Length; x++)
            {
                // If the mask char is a wildcard, ignore.
                if (stringMask[x] == '?') { continue; }

                // If the mask char is not a wildcard, check if a match is not made in the pattern.
                if
                (
                    (stringMask[x] == 'x') &&                     // Check if not wildcard.
                    (bytePattern[x] != memoryRegion[startOffset + x]) // Check if not match false.
                )
                { return false; } 
            }

            // The loop was successful, as everything matched up to this point.
            // We found the pattern.
            return true;
        }

    }
}
