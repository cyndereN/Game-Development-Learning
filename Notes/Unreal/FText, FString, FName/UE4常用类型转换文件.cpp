{
		//FString To FName
		FString Str = TEXT("TestString");
		FName Name = FName(*Str);
		//FName To FString
		FString TargetFString = Name.ToString();
	}

	{
		//std::string to FString
		std::string stdStr = "TestString";
		FString MyStr(stdStr.c_str());

		//FString To char*
		char* c = TCHAR_TO_UTF8(*MyStr);
		// FString to std::string 
		std::string TargetString(c);
	
	}

	{
		//FString to Int32
		FString Str = TEXT("TestString");
		int32 i = FCString::Atoi(*Str);
		//or
		 i = atoi(TCHAR_TO_UTF8(*Str));

		// FString to float
		int32 f = FCString::Atof(*Str);
		//or
		f = atof(TCHAR_TO_UTF8(*Str));

		//FString To bool
		bool bNewbool = Str.ToBool();
		
		// Int32 to FString
		FString TStr = FString::FromInt(123);
		// float to FString
		FString TStr1 = FString::SanitizeFloat(36.23f);
		// bool to FString
		int32 b = true;
		FString Out= b ? TEXT("true"): TEXT("false");		
	}
	{
		//FString to TCHAR*
		FString Str1(TEXT("TestString"));
		const TCHAR* MyTChar = *Str1;

		//FString to FText
		FText MyText = FText::FromString(Str1);

		//FText to FName
		FName Name = FName(*MyText.ToString());

		//FName to FText
		FText MyText1 = FText::FromName(Name);

	}
	{
		//FString To TArray<uint8>
		
		FString Str = "Hello UE4";
		
		TArray<uint8> outDataArray;

		TCHAR* CharData = Str.GetCharArray().GetData();
		int32 Len = FCString::Strlen(CharData);
		for (int i = 0; i < Len; i++)
		{
			uint8* dst = (uint8*)TCHAR_TO_UTF8(CharData);		
			CharData++;
			outDataArray.Add(*dst);
		}
		//FString To TArray<uint8>
		FBase64::Decode(Str, outDataArray);

		//TArray<uint8> To FString
		const std::string cstr(reinterpret_cast<const char*>(outDataArray.GetData()), outDataArray.Num());
		FString  MyStr1(cstr.c_str());

		//TArray<uint8> To FString
		FString DestStr = FBase64::Encode(outDataArray);


	}
	{
		// 枚举转字符串
		enum ETempEnum
		{
			One,
			Two,
			Three,
		};
		{
			//通过枚举类型的名字找到对应枚举类型的COD class default
			const UEnum* EnumPtr = FindObject<UEnum>(ANY_PACKAGE, TEXT("ETempEnum"), true);
			if (EnumPtr)
			{
				FString str = EnumPtr->GetNameStringByIndex(ETempEnum::Two);
			}
		}
		//字符串转枚举
		{	
			//通过枚举类型的名字找到对应枚举类型的COD class default
			const UEnum* EnumPtr = FindObject<UEnum>(ANY_PACKAGE, TEXT("ETempEnum"), true);
			if (EnumPtr)
			{
				ETempEnum OutEnum= (ETempEnum)EnumPtr->GetIndexByNameString(TEXT("Three"));
			}
		}
	}

	{
		//FString To Md5
		FString String("sdsafasfasf");
		FMD5::HashAnsiString(*String);

		// FString To Sha1
		FSHA1 Sha1Gen;
		Sha1Gen.Update((unsigned char*)TCHAR_TO_ANSI(*String), FCString::Strlen(*String));
		Sha1Gen.Final();

		FString Sha1String;
		for (int32 i = 0; i < 20; i++)
		{
			Sha1String += FString::Printf(TEXT("%02x"), Sha1Gen.m_digest[i]);
		}

	}