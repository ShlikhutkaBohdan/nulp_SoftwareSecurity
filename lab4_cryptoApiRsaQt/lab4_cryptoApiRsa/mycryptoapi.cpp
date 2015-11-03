#include "mycryptoapi.h"
#include <QFile>
#include <QDataStream>
#include <QMessageBox>
#include <QDebug>

#define _WIN32_WINNT 0x0400

static BYTE PrivateKeyWithExponentOfOne[] =
{
0x07, 0x02, 0x00, 0x00, 0x00, 0xA4, 0x00, 0x00,
0x52, 0x53, 0x41, 0x32, 0x00, 0x02, 0x00, 0x00,
0x01, 0x00, 0x00, 0x00, 0xAB, 0xEF, 0xFA, 0xC6,
0x7D, 0xE8, 0xDE, 0xFB, 0x68, 0x38, 0x09, 0x92,
0xD9, 0x42, 0x7E, 0x6B, 0x89, 0x9E, 0x21, 0xD7,
0x52, 0x1C, 0x99, 0x3C, 0x17, 0x48, 0x4E, 0x3A,
0x44, 0x02, 0xF2, 0xFA, 0x74, 0x57, 0xDA, 0xE4,
0xD3, 0xC0, 0x35, 0x67, 0xFA, 0x6E, 0xDF, 0x78,
0x4C, 0x75, 0x35, 0x1C, 0xA0, 0x74, 0x49, 0xE3,
0x20, 0x13, 0x71, 0x35, 0x65, 0xDF, 0x12, 0x20,
0xF5, 0xF5, 0xF5, 0xC1, 0xED, 0x5C, 0x91, 0x36,
0x75, 0xB0, 0xA9, 0x9C, 0x04, 0xDB, 0x0C, 0x8C,
0xBF, 0x99, 0x75, 0x13, 0x7E, 0x87, 0x80, 0x4B,
0x71, 0x94, 0xB8, 0x00, 0xA0, 0x7D, 0xB7, 0x53,
0xDD, 0x20, 0x63, 0xEE, 0xF7, 0x83, 0x41, 0xFE,
0x16, 0xA7, 0x6E, 0xDF, 0x21, 0x7D, 0x76, 0xC0,
0x85, 0xD5, 0x65, 0x7F, 0x00, 0x23, 0x57, 0x45,
0x52, 0x02, 0x9D, 0xEA, 0x69, 0xAC, 0x1F, 0xFD,
0x3F, 0x8C, 0x4A, 0xD0,

0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,

0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,

0x64, 0xD5, 0xAA, 0xB1,
0xA6, 0x03, 0x18, 0x92, 0x03, 0xAA, 0x31, 0x2E,
0x48, 0x4B, 0x65, 0x20, 0x99, 0xCD, 0xC6, 0x0C,
0x15, 0x0C, 0xBF, 0x3E, 0xFF, 0x78, 0x95, 0x67,
0xB1, 0x74, 0x5B, 0x60,

0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
};


MyCryptoApi::MyCryptoApi()
{
    pbBlob = NULL;
    DWORD dwResult;
    if (!CryptAcquireContext(&hCryptProv, NULL, MS_DEF_PROV, PROV_RSA_FULL, 0))
    {
        dwResult = GetLastError();
        if (dwResult == NTE_BAD_KEYSET)
        {
            if (!CryptAcquireContext(&hCryptProv,
                NULL, MS_DEF_PROV, PROV_RSA_FULL,
                CRYPT_NEWKEYSET))
            {
                dwResult = GetLastError();
                //qDebud()<<"Error [0x%x]: CryptAcquireContext() failed.";
                return;
            }
        }
        else {
            dwResult = GetLastError();
            return;
        }
    }
}

void MyCryptoApi::ImportKey(){
    HCRYPTKEY hKey;
    DWORD dwResult;
    BYTE data[256];//for key
    DWORD cbBlob = 1024;

    if (pbBlob) {
        if (!CryptImportKey(hCryptProv, pbBlob, cbBlob, 0, 0, &hSessionKey))
        {
            dwResult = GetLastError();
            //MessageBox("Error [0x%x]: CryptImportKey() failed.",
              //                            "Information", MB_OK);
            QMessageBox::information(0, "error", "Error [0x%x]: CryptImportKey() failed.1");
            return;
        }
    }
    else {
        if (!CryptImportKey(hCryptProv, PrivateKeyWithExponentOfOne,
            sizeof(PrivateKeyWithExponentOfOne), 0, 0, &hKey))
        {
            dwResult = GetLastError();
            //MessageBox("Error CryptImportKey() failed.",
              //                    "Information", MB_OK);
            QMessageBox::information(0, "error", "Error [0x%x]: CryptImportKey() failed.2");
            return;
        }
        if (!CryptGenKey(hCryptProv, CALG_RC4, CRYPT_EXPORTABLE, &hSessionKey))
        {
            dwResult = GetLastError();
            //MessageBox("Error CryptGenKey() failed.",
              //                 "Information", MB_OK);
            QMessageBox::information(0, "error", "Error CryptGenKey() failed.");
            return;
        }
    }
}

/*
void MyCryptoApi::InitApi(){

    //Clear context
    CryptAcquireContextW(&hCryptProv, L"Your product name", NULL, PROV_RSA_FULL,
      CRYPT_DELETEKEYSET);
    //Create new context
    if (!CryptAcquireContextW(&hCryptProv, L"Your product name", NULL, PROV_RSA_FULL,
      CRYPT_NEWKEYSET)) {
        QMessageBox::information(0, "error", "CryptAcquireContextW");
        //Error handling
        throw "CryptAcquireContextW error";
    }
}*/

void MyCryptoApi::Encrypt(){
    //encrypt
    QFile inputFile("e:\\1.txt");
    QFile outputFile("e:\\2.txt");
    if(!outputFile.open(QIODevice::WriteOnly)) {
        QMessageBox::information(0, "error", outputFile.errorString());
    }
    if(!inputFile.open(QIODevice::ReadOnly)) {
        QMessageBox::information(0, "error", inputFile.errorString());
    }
    QDataStream inputStream(&inputFile);
    QDataStream outputStream(&outputFile);
    char buffer[256];

    BYTE cipherBlock[256];
    //prepare to encryption
    DWORD dwResult;

    while(!inputStream.atEnd()) {
        DWORD bufferLength = inputStream.readRawData(buffer, 256);

        //encryption
        memcpy(cipherBlock, buffer, bufferLength*sizeof(char));
        if (!CryptEncrypt(hSessionKey, 0, TRUE, 0, cipherBlock, &bufferLength, bufferLength))
        {
           dwResult = GetLastError();
           //MessageBox("Error CryptEncrypt() failed.", "Information", MB_OK);
           QMessageBox::information(0,"error","Error CryptEncrypt() failed..");
           return;
        }
        memcpy(buffer, cipherBlock,  bufferLength);
        //end encryption

        outputStream.writeRawData(buffer, bufferLength);
    }
    outputFile.close();
    inputFile.close();
}

void MyCryptoApi::Decrypt(){
    //decrypt
    QFile fileInput("e:\\2.txt");
    QFile fileOutput("e:\\3.txt");
    if(!fileOutput.open(QIODevice::WriteOnly)) {
        QMessageBox::information(0, "error", fileOutput.errorString());
    }
    if(!fileInput.open(QIODevice::ReadOnly)) {
        QMessageBox::information(0, "error", fileInput.errorString());
    }
    QDataStream inputStream(&fileInput);
    QDataStream outputStream(&fileOutput);
    char buffer[256];

    BYTE cipherBlock[256];
    //prepare to encryption
    DWORD dwResult;

    while(!inputStream.atEnd()) {
        DWORD bufferLength = inputStream.readRawData(buffer, 256);

        //decryption
        memcpy(cipherBlock, buffer, bufferLength*sizeof(char));
        if (!CryptDecrypt(hSessionKey, 0, TRUE, 0, cipherBlock, &bufferLength))
        {
            dwResult = GetLastError();
            QMessageBox::information(0,"error","Error CryptDecrypt() failed.");
            return;
        }
        memcpy(buffer, cipherBlock,  bufferLength);
        //end decryption

        outputStream.writeRawData(buffer, bufferLength);
    }
    fileInput.close();
    fileOutput.close();
}
