using Microsoft.AspNetCore.Mvc;
using WebApp_Src.Applications.Services;
using WebApp_Src.Presentations.ViewModels;
using Microsoft.VisualBasic;
namespace WebApp_Src.Presentations.Controllers;

[Route("DeleteDepartment")]
public class DeleteDepartmentController : Controller
{
    private readonly ILogger<DeleteDepartmentController> _logger;
    private readonly IDeleteDepartmentService _deleteDepartmentService;
    private readonly DeleteDepartmentViewModelAdapter _adapter;
    private readonly TempDataStore<DeleteDepartmentViewModel> _deptDataStore;

    public DeleteDepartmentController(
      ILogger<DeleteDepartmentController> logger,
      IDeleteDepartmentService deleteDepartmentService,
      DeleteDepartmentViewModelAdapter deleteDepartmentViewModelAdapter,
      TempDataStore<DeleteDepartmentViewModel> deptDataStore)
    {
        _logger = logger;
        _deleteDepartmentService = deleteDepartmentService;
        _adapter = deleteDepartmentViewModelAdapter;
        _deptDataStore = deptDataStore;
    }
    [HttpGet("Confirm/{id}")]
    public IActionResult Confirm(int id)
    {
        var department = _deleteDepartmentService.GetById(id);
        var viewModel = new DeleteDepartmentViewModel
        {
            DeptId = id,
            DeptName = department?.Name
        };
        if (_deleteDepartmentService.ExistEmployee(id) == false)
        {
            ModelState.AddModelError("", "この部門には社員が存在するため削除できません");
            return View("Confirm", viewModel);
        }
        _logger.LogInformation($"Confirm.id:{id}");
        _logger.LogInformation($"Confirm.DeptId:{viewModel.DeptId}");
        return View(viewModel);
    }

    [HttpPost("Delete")]
    public IActionResult Delete(DeleteDepartmentViewModel viewModel)
    {
        _deleteDepartmentService.Delete(viewModel.DeptId);

        _deptDataStore.Save(this, viewModel);

        return RedirectToAction("Complete");
    }
    [HttpGet("Complete")]
    public IActionResult Complete()
    {
        var viewModel = _deptDataStore.Load(this);

        if (viewModel == null)
            return RedirectToAction("ShowDept");

        return View(viewModel);
    }


    // 確認画面で[戻る]ボタンを押下
    [HttpPost("Back")]
    public IActionResult Back(DeleteDepartmentViewModel viewModel)
    {
        _logger.LogInformation("[戻る]ボタンクリック:{0}", viewModel!.ToString());
        // EmployeeRegisterViewModelをシリアライズして、TempDataに保存する
        _deptDataStore.Save(this, viewModel);
        // 入力画面を出力するアクションメソッドにリダイレクトする
        return RedirectToAction("ShowDept");
    }
}