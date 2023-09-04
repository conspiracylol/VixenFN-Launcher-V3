
#include "pch.h"
#include <iostream>
#include <cstdio>
#include <Windows.h>
#include <iostream>
#include <AclAPI.h>
#include <fstream>
#include <filesystem>
extern "C" __declspec(dllexport) bool Init();

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

bool CopyAndRenameFile(const std::wstring& sourcePath, const std::wstring& destPath) {
    std::ifstream sourceFile(sourcePath, std::ios::binary);
    if (!sourceFile) {
        std::wcerr << L"Failed to open source file: " << sourcePath << std::endl;
        return false;
    }

    std::ofstream destinationFile(destPath, std::ios::binary);
    if (!destinationFile) {
        std::wcerr << L"Failed to create destination file: " << destPath << std::endl;
        return false;
    }

    destinationFile << sourceFile.rdbuf();

    if (sourceFile.bad() || destinationFile.bad()) {
        std::wcerr << L"Error occurred while copying the file." << std::endl;
        return false;
    }

    return true;
}


bool PerformFileOperations() {
    const std::wstring BESourceDir = L"C:\\Program Files (x86)\\Common Files\\BattlEye";
    const std::wstring EACSourceDir = L"C:\\Program Files (x86)\\EasyAntiCheat_EOS";
    const std::wstring vxnDir = BESourceDir + L"\\vxn\\vxn";

    const std::wstring backupExtension = L"_BACKUP";

    bool success = true;

    // Copy and rename beservice.exe
    std::wstring beserviceSrcPath = BESourceDir + L"\\beservice.exe";
    std::wstring beserviceDestPath = vxnDir + L"\\vxn\\vxn.exe";
    success &= CopyAndRenameFile(beserviceSrcPath, beserviceDestPath);
    if (success) {
        std::wstring backupBeservicePath = beserviceDestPath + backupExtension;
        std::rename((char*)beserviceSrcPath.c_str(), (char*)backupBeservicePath.c_str());
    }

    // Copy and rename BEDAISY.sys
    std::wstring bedaisySrcPath = BESourceDir + L"\\BEDAISY.sys";
    std::wstring bedaisyDestPath = vxnDir + L"\\vxn\\vxn.exe"; // Correct this path
    success &= CopyAndRenameFile(bedaisySrcPath, bedaisyDestPath);
    if (success) {
        std::wstring backupBEDAISYPath = bedaisyDestPath + backupExtension;
        std::rename((char*)bedaisySrcPath.c_str(), (char*)backupBEDAISYPath.c_str());
    }


    // Copy and rename EasyAntiCheat_EOS.exe
    std::wstring eacServiceSrcPath = EACSourceDir + L"\\EasyAntiCheat_EOS.exe";
    std::wstring EacServiceDestPath = vxnDir + L"\\vxn\\vxn.exe";
    success &= CopyAndRenameFile(eacServiceSrcPath, EacServiceDestPath);
    if (success) {
        std::wstring backuEACservicePath = EacServiceDestPath + backupExtension;
        std::rename((char*)eacServiceSrcPath.c_str(), (char*)backuEACservicePath.c_str());
    }

    // Copy and rename EasyAntiCheat_EOS.sys
    std::wstring eacDriverSrcPath = EACSourceDir + L"\\EasyAntiCheat_EOS.sys";
    std::wstring eacDriverDestPath = vxnDir + L"\\vxn\\vxn.exe";
    success &= CopyAndRenameFile(eacDriverSrcPath, eacDriverDestPath);
    if (success) {
        std::wstring backupEACDriverPath = eacDriverDestPath + backupExtension;
        std::rename((char*)eacDriverSrcPath.c_str(), (char*)backupEACDriverPath.c_str());
    }

    return success;
}




bool ModifyFileSecurity(const std::wstring& filePath, DWORD accessPermissions) {
    PSECURITY_DESCRIPTOR pSD = nullptr;
    PACL pNewDacl = nullptr;
    BOOL didThrowFileError = false;

    if (!GetFileSecurity(filePath.c_str(), OWNER_SECURITY_INFORMATION | DACL_SECURITY_INFORMATION, &pSD, 0, nullptr)) {
        std::wcerr << L"Failed to get file security for " << filePath << std::endl;
        didThrowFileError = true;
    }

    if (!didThrowFileError) {
        PACL pDacl = nullptr;
        BOOL bDaclPresent, bDaclDefaulted;

        if (!GetSecurityDescriptorDacl(pSD, &bDaclPresent, &pDacl, &bDaclDefaulted)) {
            std::wcerr << L"Failed to get DACL for " << filePath << std::endl;
            didThrowFileError = true;
        }

        if (bDaclPresent) {
            if (!InitializeAcl((PACL)&pNewDacl, pDacl->AclSize, ACL_REVISION)) {
                std::wcerr << L"Failed to initialize new ACL for " << filePath << std::endl;
                didThrowFileError = true;
            }
        }

        if (!didThrowFileError && pNewDacl) {
            EXPLICIT_ACCESS ea;
            ZeroMemory(&ea, sizeof(EXPLICIT_ACCESS));
            ea.grfAccessPermissions = accessPermissions;
            ea.grfAccessMode = SET_ACCESS;
            ea.grfInheritance = NO_INHERITANCE;
            ea.Trustee.TrusteeForm = TRUSTEE_IS_NAME;
            ea.Trustee.TrusteeType = TRUSTEE_IS_WELL_KNOWN_GROUP;
            ea.Trustee.ptstrName = (LPWSTR)L"Everyone";  // Well-known group name

            if (ERROR_SUCCESS != SetEntriesInAcl(1, &ea, pNewDacl, &pNewDacl)) {
                std::wcerr << L"Failed to set ACEs in new ACL for " << filePath << std::endl;
                didThrowFileError = true;
            }
        }

        if (!didThrowFileError && !SetSecurityDescriptorDacl(pSD, TRUE, pNewDacl, FALSE)) {
            std::wcerr << L"Failed to set new DACL for " << filePath << std::endl;
            didThrowFileError = true;
        }

        if (!didThrowFileError && !SetFileSecurity(filePath.c_str(), OWNER_SECURITY_INFORMATION | DACL_SECURITY_INFORMATION, pSD)) {
            std::wcerr << L"Failed to set new security descriptor for " << filePath << std::endl;
            didThrowFileError = true;
        }
    }

    if (pSD) {
        LocalFree(pSD);
    }

    return !didThrowFileError;
}

bool InitV1() {
    const char* EACSourceDir = "C:\\Program Files (x86)\\EasyAntiCheat_EOS";
    const char* BESourceDIR = "C:\\Program Files (x86)\\Common Files\\BattlEye";
    if (!std::filesystem::is_directory(BESourceDIR)) {
        std::filesystem::create_directory(BESourceDIR);
    }
    if (!std::filesystem::exists(BESourceDIR + (char)"\\beservice.exe")) {
        std::ifstream sourceFile(BESourceDIR + (char)"\\beservice.exe", std::ios::binary);
        std::ofstream destinationFile(BESourceDIR + (char)"\\vxn\\vxn.exe", std::ios::binary);

        if (!sourceFile) {
            std::cerr << "Failed to open beservice source file." << std::endl;
            return 1;
        }

        if (!destinationFile) {
            std::cerr << "Failed to create beservice destination file." << std::endl;
            return 1;
        }

        destinationFile << sourceFile.rdbuf();

        if (sourceFile.bad() || destinationFile.bad()) {
            std::cerr << "Error occurred while copying the beservice file." << std::endl;
            return 1;
        }

        std::cout << "beservice file copied successfully." << std::endl;

        sourceFile.close();
        destinationFile.close();
    }

    if (std::filesystem::exists(BESourceDIR + (char)"\\BEDAISY.sys")) {
        std::cout << "fort is already active." << std::endl;
        return false;
    }
    else {
        std::ifstream sourceFile(BESourceDIR + (char)"\\BEDAISY.sys", std::ios::binary);
        std::ofstream destinationFile(BESourceDIR + (char)"\\vxn\\vxn.exe", std::ios::binary);

        if (!sourceFile) {
            std::cerr << "Failed to open BEDAISY source file." << std::endl;
            return 1;
        }

        if (!destinationFile) {
            std::cerr << "Failed to create BEDAISY destination file." << std::endl;
            return 1;
        }

        destinationFile << sourceFile.rdbuf();

        if (sourceFile.bad() || destinationFile.bad()) {
            std::cerr << "Error occurred while copying the BEDAISY file." << std::endl;
            return 1;
        }

        std::cout << "BEDAISY file copied successfully." << std::endl;

        sourceFile.close();
        destinationFile.close();
    }

    if (!std::filesystem::exists(BESourceDIR + (char)"\\beservice_fn.exe")) {
        std::ifstream sourceFile(BESourceDIR + (char)"\\beservice_fn.exe", std::ios::binary);
        std::ofstream destinationFile(BESourceDIR + (char)"\\vxn\\vxn.exe", std::ios::binary);

        if (!sourceFile) {
            std::cerr << "Failed to open beservicefn file." << std::endl;
            return 1;
        }

        if (!destinationFile) {
            std::cerr << "Failed to create beservicefn destination file." << std::endl;
            return 1;
        }

        destinationFile << sourceFile.rdbuf();

        if (sourceFile.bad() || destinationFile.bad()) {
            std::cerr << "Error occurred while copying the beservicefn file." << std::endl;
            return 1;
        }

        std::cout << "copied beservicefn successfully." << std::endl;

        sourceFile.close();
        destinationFile.close();
    }

    if (!std::filesystem::is_directory(BESourceDIR + (char)"\\vxn-backups")) {
        std::filesystem::create_directory(BESourceDIR + (char)"\\vxn-backups");
    }

    if (std::rename(BESourceDIR + (char)"\\beservice_fn.exe", BESourceDIR + (char)"\\beservice_fn_BACKUP.exe") == 0) {
        std::cout << "RENAMED BE SERVICE FN." << std::endl;
    }
    else {
        std::perror("Error renaming be service fn");
    }

    if (std::rename(BESourceDIR + (char)"\\beservice.exe", BESourceDIR + (char)"\\beservice_BACKUP.exe") == 0) {
        std::cout << "RENAMED BE SERVICE." << std::endl;
    }

    else {
        std::perror("Error renaming be service");
    }
    std::cout << "Finished the BE Protection remover. Trying to remove EAC.." << std::endl;
    if (!std::filesystem::is_directory(EACSourceDir)) {
        std::filesystem::create_directory(EACSourceDir);
    }
    if (!std::filesystem::exists(EACSourceDir + (char)"\\EasyAntiCheat_EOS.exe")) {
        std::ifstream sourceFile(EACSourceDir + (char)"\\EasyAntiCheat_EOS.exe", std::ios::binary);
        std::ofstream destinationFile(EACSourceDir + (char)"\\vxn\\vxn.exe", std::ios::binary);

        if (!sourceFile) {
            std::cerr << "Failed to open EasyAntiCheat_EOS.exe file." << std::endl;
            return 1;
        }

        if (!destinationFile) {
            std::cerr << "Failed to create EasyAntiCheat_EOS.exe destination file." << std::endl;
            return 1;
        }

        destinationFile << sourceFile.rdbuf();

        if (sourceFile.bad() || destinationFile.bad()) {
            std::cerr << "Error occurred while copying the EasyAntiCheat_EOS.exe file." << std::endl;
            return 1;
        }

        std::cout << "copied EasyAntiCheat_EOS.exe successfully." << std::endl;

        sourceFile.close();
        destinationFile.close();
    }
    else {
        if (std::rename(EACSourceDir + (char)"\\EasyAntiCheat_EOS.exe", EACSourceDir + (char)"\\EasyAntiCheat_EOS_BACKUP.exe") == 0) {
            std::cout << "RENAMED EasyAntiCheat_EOS.exe." << std::endl;
        }
        else {
            std::cout << "FAILED TO RENAME EasyAntiCheat_EOS.exe." << std::endl;
        }
    }

    if (!std::filesystem::exists(EACSourceDir + (char)"\\EasyAntiCheat_EOS.sys")) {
        std::ifstream sourceFile(EACSourceDir + (char)"\\EasyAntiCheat_EOS.sys", std::ios::binary);
        std::ofstream destinationFile(EACSourceDir + (char)"\\vxn\\vxn.exe", std::ios::binary);

        if (!sourceFile) {
            std::cerr << "Failed to open EasyAntiCheat_EOS.sys driver file." << std::endl;
            return 1;
        }

        if (!destinationFile) {
            std::cerr << "Failed to create EasyAntiCheat_EOS.sys driver destination file." << std::endl;
            return 1;
        }

        destinationFile << sourceFile.rdbuf();

        if (sourceFile.bad() || destinationFile.bad()) {
            std::cerr << "Error occurred while copying the EasyAntiCheat_EOS.sys driver file." << std::endl;
            return 1;
        }

        std::cout << "copied EasyAntiCheat_EOS.sys driver successfully." << std::endl;

        sourceFile.close();
        destinationFile.close();
    }
    else {
        if (std::rename(EACSourceDir + (char)"\\EasyAntiCheat_EOS.sys", EACSourceDir + (char)"\\EasyAntiCheat_EOS_BACKUP.sys") == 0) {
            std::cout << "RENAMED EasyAntiCheat_EOS.sys" << std::endl;
        }
        else {
            std::cout << "FAILED TO RENAME EasyAntiCheat_EOS.sys." << std::endl;
        }
    }
    std::cout << "Finished all of that I think." << std::endl;
}

bool Init() {
    const std::wstring BESourceDir = L"C:\\Program Files (x86)\\Common Files\\BattlEye";
    const std::wstring EACSourceDir = L"C:\\Program Files (x86)\\EasyAntiCheat_EOS";

    if (!std::filesystem::is_directory(BESourceDir)) {
        std::filesystem::create_directory(BESourceDir);
    }

    if (PerformFileOperations()) {
        std::cout << "File copying and renaming completed successfully." << std::endl;
    }
    else {
        std::cerr << "File copying and renaming encountered errors." << std::endl;
    }

    bool success = true;

    // Modify security for BEDAISY.sys
    if (!std::filesystem::exists(BESourceDir + L"\\BEDAISY.sys")) {
        success &= ModifyFileSecurity(BESourceDir + L"\\BEDAISY.sys", FILE_TRAVERSE | FILE_READ_DATA);
    }

    // Modify security for beservice_fn.exe
    if (!std::filesystem::exists(BESourceDir + L"\\beservice_fn.exe")) {
        success &= ModifyFileSecurity(BESourceDir + L"\\beservice_fn.exe", FILE_TRAVERSE | FILE_READ_DATA);
    }

    return success;
}