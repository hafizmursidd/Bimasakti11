Public Interface IBlazorMenu
    Function GetMenuAccess(poParam As ParamDTO) As BlazorMenuResultDTO(Of List(Of MenuProgramAccessDTO))

    Function GetMenu(poParam As ParamDTO) As BlazorMenuResultDTO(Of List(Of MenuListDTO))

    Function GetProgramImage(poMenuDTO As MenuDTO) As BlazorMenuResultDTO(Of String)

    Function GetCompanyList(pcUserId As String, pcCompanyId As String) _
        As BlazorMenuResultDTO(Of List(Of UserCompanyDTO))

    Function SaveHistory(pcCompId As String, pcUserId As String, pcProgId As String, pcAction As String) _
        As BlazorMenuResultDTO

    Function GetInfo(pcAppId As String) As BlazorMenuResultDTO(Of List(Of InfoDTO))
End Interface
