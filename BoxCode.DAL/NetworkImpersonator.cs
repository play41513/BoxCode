using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

public class NetworkImpersonator : IDisposable
{
    private readonly WindowsImpersonationContext _impersonationContext;

    // P/Invoke for LogonUser
    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

    // P/Invoke for CloseHandle
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private extern static bool CloseHandle(IntPtr handle);

    private const int LOGON32_LOGON_NETWORK = 3;
    private const int LOGON32_PROVIDER_DEFAULT = 0;

    /// <summary>
    /// 開始模擬指定的使用者。
    /// </summary>
    /// <param name="userName">NAS 使用者名稱</param>
    /// <param name="password">NAS 密碼</param>
    /// <param name="domain">通常是 NAS 的主機名稱或 IP 位址，如果 NAS 是網域的一部分，則為網域名稱</param>
    public NetworkImpersonator(string userName, string password, string domain)
    {
        IntPtr tokenHandle = IntPtr.Zero;
        bool success = LogonUser(userName, domain, password, LOGON32_LOGON_NETWORK, LOGON32_PROVIDER_DEFAULT, out tokenHandle);

        if (!success)
        {
            int errorCode = Marshal.GetLastWin32Error();
            throw new Win32Exception(errorCode, "無法登入網路使用者。請檢查帳號、密碼或網域(IP)是否正確。");
        }

        WindowsIdentity identity = new WindowsIdentity(tokenHandle);
        _impersonationContext = identity.Impersonate();

        // 登入後就可以關閉 token handle
        CloseHandle(tokenHandle);
    }

    /// <summary>
    /// 停止模擬並恢復原來的身分。
    /// </summary>
    public void Dispose()
    {
        _impersonationContext?.Undo();
    }
}