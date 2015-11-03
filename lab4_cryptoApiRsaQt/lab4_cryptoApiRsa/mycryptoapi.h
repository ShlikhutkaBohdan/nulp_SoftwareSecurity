#ifndef MYCRYPTOAPI_H
#define MYCRYPTOAPI_H

#include <Windows.h>
#include <WinCrypt.h>

class MyCryptoApi
{
public:
    MyCryptoApi();
    void Encrypt();
    void Decrypt();

    //void InitApi();
    void ImportKey();
private:

    void MyCryptoApi::GenerateKey(int keyLengthInBits);

    HCRYPTPROV hCryptProv;
    BYTE *pbBlob;
    HCRYPTKEY hSessionKey;
};

#endif // MYCRYPTOAPI_H
