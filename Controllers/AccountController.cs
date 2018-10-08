//using System;
//using System.Globalization;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Mvc;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin.Security;
//using for_copying.Models;

//namespace for_copying.controllers
//{
//    [Authorize]
//    public class AccountController : Controller
//    {
//        private ApplicationSignInManager _signInManager;
//        private ApplicationUserManager _userManager;

//        public AccountController()
//        {
//        }

//        public AccountController(ApplicationUserManager usermanager, ApplicationSignInManager signinmanager)
//        {
//            usermanager = usermanager;
//            signinmanager = signinmanager;
//        }

//        public ApplicationUserManager signinmanager
//        {
//            get
//            {
//                return _signinmanager ?? HttpContext.getowincontext().get<applicationsigninmanager>();
//            }
//            private set
//            {
//                _signinmanager = value;
//            }
//        }

//        public ApplicationUserManager usermanager
//        {
//            get
//            {
//                return _usermanager ?? HttpContext.getowincontext().getusermanager<ApplicationUserManager>();
//            }
//            private set
//            {
//                _usermanager = value;
//            }
//        }

//        //
//        // get: /account/login
//        [AllowAnonymous]
//        public ActionResult login(string returnurl)
//        {
//            ViewBag.returnurl = returnurl;
//            return View();
//        }

//        //
//        // post: /account/login
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> login(LoginViewModel model, string returnurl)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }

//            // сбои при входе не приводят к блокированию учетной записи
//            // чтобы ошибки при вводе пароля инициировали блокирование учетной записи, замените на shouldlockout: true
//            var result = await signinmanager.passwordsigninasync(model.Email, model.Password, model.RememberMe, shouldlockout: false);
//            switch (result)
//            {
//                case SignInStatus.Success:
//                    return redirecttolocal(returnurl);
//                case SignInStatus.LockedOut:
//                    return View("lockout");
//                case SignInStatus.RequiresVerification:
//                    return RedirectToAction("sendcode", new { returnurl = returnurl, rememberme = model.RememberMe });
//                case SignInStatus.Failure:
//                default:
//                    ModelState.AddModelError("", "неудачная попытка входа.");
//                    return View(model);
//            }
//        }

//        //
//        // get: /account/verifycode
//        [AllowAnonymous]
//        public async Task<ActionResult> verifycode(string provider, string returnurl, bool rememberme)
//        {
//            // требовать предварительный вход пользователя с помощью имени пользователя и пароля или внешнего имени входа
//            if (!await signinmanager.hasbeenverifiedasync())
//            {
//                return View("error");
//            }
//            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnurl, RememberMe = rememberme });
//        }

//        //
//        // post: /account/verifycode
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> verifycode(VerifyCodeViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }

//            // приведенный ниже код защищает от атак методом подбора, направленных на двухфакторные коды. 
//            // если пользователь введет неправильные коды за указанное время, его учетная запись 
//            // будет заблокирована на заданный период. 
//            // параметры блокирования учетных записей можно настроить в identityconfig
//            var result = await signinmanager.twofactorsigninasync(model.Provider, model.Code, ispersistent: model.RememberMe, rememberbrowser: model.RememberBrowser);
//            switch (result)
//            {
//                case SignInStatus.Success:
//                    return redirecttolocal(model.ReturnUrl);
//                case SignInStatus.LockedOut:
//                    return View("lockout");
//                case SignInStatus.Failure:
//                default:
//                    ModelState.AddModelError("", "неправильный код.");
//                    return View(model);
//            }
//        }

//        //
//        // get: /account/register
//        [AllowAnonymous]
//        public ActionResult register()
//        {
//            return View();
//        }

//        //
//        // post: /account/register
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> register(RegisterViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = new ApplicationUser { username = model.Email, email = model.Email };
//                var result = await usermanager.createasync(user, model.Password);
//                if (result.succeeded)
//                {
//                    await signinmanager.signinasync(user, ispersistent: false, rememberbrowser: false);

//                    // дополнительные сведения о том, как включить подтверждение учетной записи и сброс пароля, см. по адресу: http://go.microsoft.com/fwlink/?linkid=320771
//                    // отправка сообщения электронной почты с этой ссылкой
//                    // string code = await usermanager.generateemailconfirmationtokenasync(user.id);
//                    // var callbackurl = url.action("confirmemail", "account", new { userid = user.id, code = code }, protocol: request.url.scheme);
//                    // await usermanager.sendemailasync(user.id, "подтверждение учетной записи", "подтвердите вашу учетную запись, щелкнув <a href=\"" + callbackurl + "\">здесь</a>");

//                    return RedirectToAction("index", "home");
//                }
//                adderrors(result);
//            }

//            // появление этого сообщения означает наличие ошибки; повторное отображение формы
//            return View(model);
//        }

//        //
//        // get: /account/confirmemail
//        [AllowAnonymous]
//        public async Task<ActionResult> confirmemail(string userid, string code)
//        {
//            if (userid == null || code == null)
//            {
//                return View("error");
//            }
//            var result = await usermanager.confirmemailasync(userid, code);
//            return View(result.succeeded ? "confirmemail" : "error");
//        }

//        //
//        // get: /account/forgotpassword
//        [AllowAnonymous]
//        public ActionResult forgotpassword()
//        {
//            return View();
//        }

//        //
//        // post: /account/forgotpassword
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> forgotpassword(ForgotPasswordViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = await usermanager.findbynameasync(model.Email);
//                if (user == null || !(await usermanager.isemailconfirmedasync(user.id)))
//                {
//                    // не показывать, что пользователь не существует или не подтвержден
//                    return View("forgotpasswordconfirmation");
//                }

//                // дополнительные сведения о том, как включить подтверждение учетной записи и сброс пароля, см. по адресу: http://go.microsoft.com/fwlink/?linkid=320771
//                // отправка сообщения электронной почты с этой ссылкой
//                // string code = await usermanager.generatepasswordresettokenasync(user.id);
//                // var callbackurl = url.action("resetpassword", "account", new { userid = user.id, code = code }, protocol: request.url.scheme);		
//                // await usermanager.sendemailasync(user.id, "сброс пароля", "сбросьте ваш пароль, щелкнув <a href=\"" + callbackurl + "\">здесь</a>");
//                // return redirecttoaction("forgotpasswordconfirmation", "account");
//            }

//            // появление этого сообщения означает наличие ошибки; повторное отображение формы
//            return View(model);
//        }

//        //
//        // get: /account/forgotpasswordconfirmation
//        [AllowAnonymous]
//        public ActionResult forgotpasswordconfirmation()
//        {
//            return View();
//        }

//        //
//        // get: /account/resetpassword
//        [AllowAnonymous]
//        public ActionResult resetpassword(string code)
//        {
//            return code == null ? View("error") : View();
//        }

//        //
//        // post: /account/resetpassword
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> resetpassword(ResetPasswordViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }
//            var user = await usermanager.findbynameasync(model.Email);
//            if (user == null)
//            {
//                // не показывать, что пользователь не существует
//                return RedirectToAction("resetpasswordconfirmation", "account");
//            }
//            var result = await usermanager.resetpasswordasync(user.id, model.Code, model.Password);
//            if (result.succeeded)
//            {
//                return RedirectToAction("resetpasswordconfirmation", "account");
//            }
//            adderrors(result);
//            return View();
//        }

//        //
//        // get: /account/resetpasswordconfirmation
//        [AllowAnonymous]
//        public ActionResult resetpasswordconfirmation()
//        {
//            return View();
//        }

//        //
//        // post: /account/externallogin
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public ActionResult externallogin(string provider, string returnurl)
//        {
//            // запрос перенаправления к внешнему поставщику входа
//            return new challengeresult(provider, Url.Action("externallogincallback", "account", new { returnurl = returnurl }));
//        }

//        //
//        // get: /account/sendcode
//        [AllowAnonymous]
//        public async Task<ActionResult> SendCode(string returnurl, bool rememberme)
//        {
//            var userid = await signinmanager.getverifieduseridasync();
//            if (userid == null)
//            {
//                return View("error");
//            }
//            var userfactors = await usermanager.getvalidtwofactorprovidersasync(userid);
//            var factoroptions = userfactors.select(purpose => new selectlistitem { text = purpose, value = purpose }).tolist();
//            return View(new SendCodeViewModel { Providers = factoroptions, ReturnUrl = returnurl, RememberMe = rememberme });
//        }

//        //
//        // post: /account/sendcode
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> sendcode(SendCodeViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View();
//            }

//            // создание и отправка маркера
//            if (!await signinmanager.sendtwofactorcodeasync(model.SelectedProvider))
//            {
//                return View("error");
//            }
//            return RedirectToAction("verifycode", new { provider = model.SelectedProvider, returnurl = model.ReturnUrl, rememberme = model.RememberMe });
//        }

//        //
//        // get: /account/externallogincallback
//        [AllowAnonymous]
//        public async Task<ActionResult> externallogincallback(string returnurl)
//        {
//            var logininfo = await authenticationmanager.GetExternalLoginInfoAsync();
//            if (logininfo == null)
//            {
//                return RedirectToAction("login");
//            }

//            // выполнение входа пользователя посредством данного внешнего поставщика входа, если у пользователя уже есть имя входа
//            var result = await signinmanager.externalsigninasync(logininfo, ispersistent: false);
//            switch (result)
//            {
//                case SignInStatus.Success:
//                    return redirecttolocal(returnurl);
//                case SignInStatus.LockedOut:
//                    return View("lockout");
//                case SignInStatus.RequiresVerification:
//                    return RedirectToAction("sendcode", new { returnurl = returnurl, rememberme = false });
//                case SignInStatus.Failure:
//                default:
//                    // если у пользователя нет учетной записи, то ему предлагается создать ее
//                    ViewBag.returnurl = returnurl;
//                    ViewBag.loginprovider = logininfo.Login.LoginProvider;
//                    return View("externalloginconfirmation", new ExternalLoginConfirmationViewModel { Email = logininfo.Email });
//            }
//        }

//        //
//        // post: /account/externalloginconfirmation
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> externalloginconfirmation(ExternalLoginConfirmationViewModel model, string returnurl)
//        {
//            if (User.Identity.IsAuthenticated)
//            {
//                return RedirectToAction("index", "manage");
//            }

//            if (ModelState.IsValid)
//            {
//                // получение сведений о пользователе от внешнего поставщика входа
//                var info = await authenticationmanager.GetExternalLoginInfoAsync();
//                if (info == null)
//                {
//                    return View("externalloginfailure");
//                }
//                var user = new ApplicationUser { username = model.Email, email = model.Email };
//                var result = await usermanager.createasync(user);
//                if (result.succeeded)
//                {
//                    result = await usermanager.addloginasync(user.id, info.Login);
//                    if (result.succeeded)
//                    {
//                        await signinmanager.signinasync(user, ispersistent: false, rememberbrowser: false);
//                        return redirecttolocal(returnurl);
//                    }
//                }
//                adderrors(result);
//            }

//            ViewBag.returnurl = returnurl;
//            return View(model);
//        }

//        //
//        // post: /account/logoff
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult logoff()
//        {
//            authenticationmanager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
//            return RedirectToAction("index", "home");
//        }

//        //
//        // get: /account/externalloginfailure
//        [AllowAnonymous]
//        public ActionResult externalloginfailure()
//        {
//            return View();
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (_usermanager != null)
//                {
//                    _usermanager.dispose();
//                    _usermanager = null;
//                }

//                if (_signinmanager != null)
//                {
//                    _signinmanager.dispose();
//                    _signinmanager = null;
//                }
//            }

//            base.Dispose(disposing);
//        }

//        #region вспомогательные приложения
//        // используется для защиты от xsrf-атак при добавлении внешних имен входа
//        private const string xsrfkey = "xsrfid";

//        private IAuthenticationManager authenticationmanager
//        {
//            get
//            {
//                return HttpContext.GetOwinContext().authentication;
//            }
//        }

//        private void adderrors(IdentityResult result)
//        {
//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError("", error);
//            }
//        }

//        private ActionResult redirecttolocal(string returnurl)
//        {
//            if (Url.IsLocalUrl(returnurl))
//            {
//                return Redirect(returnurl);
//            }
//            return RedirectToAction("index", "home");
//        }

//        internal class challengeresult : HttpUnauthorizedResult
//        {
//            public challengeresult(string provider, string redirecturi)
//                : this(provider, redirecturi, null)
//            {
//            }

//            public challengeresult(string provider, string redirecturi, string userid)
//            {
//                loginprovider = provider;
//                redirecturi = redirecturi;
//                userid = userid;
//            }

//            public string loginprovider { get; set; }
//            public string redirecturi { get; set; }
//            public string userid { get; set; }

//            public override void ExecuteResult(ControllerContext context)
//            {
//                var properties = new AuthenticationProperties { RedirectUri = redirecturi };
//                if (userid != null)
//                {
//                    properties.Dictionary[xsrfkey] = userid;
//                }
//                context.HttpContext.GetOwinContext().Authentication.challenge(properties, loginprovider);
//            }
//        }
//        #endregion
//    }
//}