class DepositUI{
    public:
        virtual void RequestDepositAmount() = 0;
};

class DepositTransaction: public Transaction{
    public:
        DepositTransaction(DepositUI& ui): itsDepositUI(ui){}

        virtual void Execute(){
            //...
            itsDepositUI.RequestDepositAmount();
            //...
        }
}

// WithdrawlUI, TransferUI are similar

class UI: public DepositUI
        , public WithdrawlUI
        , public TransferUI
{
    public:
        virtual void RequestDepositAmount();
        virtual void RequestWithdrawlAmount();
        virtual void RequestTransferAmount();
}

// 对以上LSP程序检查，每个transaction都必须知道UI，可以使用以下惯用法(idiom)

// 接口初始化用法
UI Gui; // global object
void f(){
    DepositTransaction dt(Gui);
}

// 分离的全局指针
// in some module that gets linked in
// to the rest of the app
static UI Lui; // non-global object
DepositUI& GDepositUI = Lui;
WithdrawlUI& GWithdrawalUI = Lui;
TransferUI& GTransferUI = Lui;

// in depositTransaction.h module

class DepositTransaction: public Transaction{
    public:

        virtual void Execute(){
            //...
            GDeposiUI.RequestDepositAmount();
            //...
        }
}

// 不要包装到一个类中，这样改动任意一个接口又要重新编译“ui_globals.h"，这样又把接口合并了