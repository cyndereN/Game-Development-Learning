
/// Configures which anim set failures should be considered an error
USTRUCT(BlueprintType)
struct FObjectSetErrorFlags
{
	GENERATED_BODY()
public:

	static const FObjectSetErrorFlags All;
	static const FObjectSetErrorFlags None;

	constexpr FObjectSetErrorFlags() = default;

	constexpr FObjectSetErrorFlags(const bool bAllFlags)
		: bMissingObjectSet(bAllFlags)
		, bUnresolvedAsset(bAllFlags)
	{
	}

	/// Error when we fail to find an object set
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	bool bMissingObjectSet = true;

	/// Error when we fail to get/resolve an asset from our object set
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	bool bUnresolvedAsset = true;
};


// ---------------------------------

const FObjectSetErrorFlags FObjectSetErrorFlags::All{ true };
const FObjectSetErrorFlags FObjectSetErrorFlags::None{ false };


FObjectSetErrorFlags ErrorFlags = FObjectSetErrorFlags::None;
ErrorFlags.bUnresolvedAsset = true;