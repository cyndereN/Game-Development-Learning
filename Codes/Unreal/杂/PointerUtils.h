#pragma once

PUSH_RELAXED_COMPILER_WARNING_LEVEL
#include <type_traits>
POP_RELAXED_COMPILER_WARNING_LEVEL


/**
 *	Templates for taking the addresses of non-pointers but passing through things which are already pointers.
 *	
 *	Pointers to pointers are only supported through explicit template parameterisation - if the user doesn't
 *	specify that they want a pointer to a pointer, AsPointer will return a single-level pointer as-is.
 *	
 *	//E.g. Given the following declarations:
 *	T* Ptr = SomePointer;
 *	T** PtrPtr = &Ptr;
 *	//These calls will return a T* and a T** respectively. (This mismatch is unintended)
 *	AsPointer(Ptr);
 *	AsPointer(PtrPtr);
 *	//Whereas these calls will both return the intended T**.
 *	AsPointer<T*>(Ptr)
 *	AsPointer<T*>(PtrPtr);
 */
namespace PointerUtils
{
	/// Returns a pointer to Data, or Data if Data's already a pointer
	template<typename T>
	T* AsPointer(T* Data) { return Data; }

	template<typename T>
	T* AsPointer(T& Data) { return std::addressof(Data); }

	template<typename T>
	void* AsPointer(T** Data)
	{
		static_assert(!std::is_same<T, T>::value,
			"Type deduction is not supported for pointers-to-pointers. "
			"Use AsPointer<T*>(PtrToPtr) instead of AsPointer(PtrToPtr).");
		return nullptr;
	}

	/// Defines a type "pointer to T" or just "T" if T is already a pointer
	template<typename T>
	using TPointer = decltype(AsPointer(*static_cast<T*>(nullptr)));

	/// Turns a pointer or reference to any value into a pointer or reference to a const value
	template<typename T>
	using TPointerToConst = const std::remove_pointer_t<TPointer<T>>*;
}


/// Fetch reflection info from a USTRUCT or UCLASS
// Consider using further sfinae magic to support non-reflected types.
template<typename T>
decltype(T::StaticStruct()) StaticStructOrClass()
{
	return T::StaticStruct();
}

template<typename T>
decltype(T::StaticClass()) StaticStructOrClass()
{
	return T::StaticClass();
}


/// Checks and dereferences an instance of a UCLASS or USTRUCT
template<typename T>
T& Dereference(T* Pointer)
{
	ASSERT(Pointer, TEXT("Attempted to dereference null pointer of type %s."), *StaticStructOrClass<T>()->GetName());
	return *Pointer;
}


/// Dereferences something if it's non-null, otherwise returning Default
template<typename T>
T& Dereference(TObjectPtr<T> Pointer)
{
	ASSERT(Pointer, TEXT("Attempted to dereference null pointer of type %s."), *StaticStructOrClass<T>()->GetName());
	return *Pointer;
}

/// Dereferences something if it's non-null, otherwise returning Default
template<typename T>
T& DereferenceOr(T* Pointer, T& Default)
{
	return Pointer ? *Pointer : Default;
}

template<typename T>
T DereferenceOr(const T* Pointer, T&& Default)
{
	return Pointer ? *Pointer : Default;
}


/// Removes a level of indirection from a pointer-to-pointer by dereferencing it where possible, otherwise returning null
template<typename T>
T* DereferenceOrNull(T* const* const PointerToPointer)
{
	return DereferenceOr(PointerToPointer, { });
}
