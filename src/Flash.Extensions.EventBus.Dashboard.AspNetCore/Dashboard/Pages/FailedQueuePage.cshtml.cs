#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Flash.Extensions.EventBus.Dashboard.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    #line 2 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
    using Flash.Extensions.EventBus.Dashboard;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
    using Flash.Extensions.EventBus.Dashboard.Pages;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
    using Flash.Extensions.EventBus.Dashboard.Resources;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    internal partial class FailedQueuePage : RazorPage
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");






            
            #line 6 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
  
    Layout = new LayoutPage(Strings.NavigationMenu_Queues);
    var list = this.Monitor.GetFailedQueues().ConfigureAwait(false).GetAwaiter().GetResult();



            
            #line default
            #line hidden
WriteLiteral("\r\n<div class=\"row\">\r\n    <div class=\"col-md-3\">\r\n        ");


            
            #line 14 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
   Write(Html.QueueSidebar());

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div class=\"col-md-9\">\r\n        <h1 class=\"page-header\">");


            
            #line 17 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
                           Write(Strings.QueuesPage_FailedQueues);

            
            #line default
            #line hidden
WriteLiteral("</h1>\r\n");


            
            #line 18 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
         if (list.Count == 0)
        {

            
            #line default
            #line hidden
WriteLiteral("            <div class=\"alert alert-success\">\r\n                ");


            
            #line 21 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
           Write(Strings.QueuesPage_NoQueues);

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");


            
            #line 23 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
        }
        else
        {

            
            #line default
            #line hidden
WriteLiteral(@"            <div class=""js-jobs-list"">
                <div class=""table-responsive"">
                    <table class=""table table-striped"">
                        <thead>
                            <tr>
                                <th class=""min-width-200p"">");


            
            #line 31 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
                                                      Write(Strings.QueuesPage_Table_Queue);

            
            #line default
            #line hidden
WriteLiteral("</th>\r\n                                <th>");


            
            #line 32 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
                               Write(Strings.QueuesPage_Table_Messages);

            
            #line default
            #line hidden
WriteLiteral("</th>\r\n                            </tr>\r\n                        </thead>\r\n     " +
"                   <tbody>\r\n");


            
            #line 36 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
                             foreach (var queue in list)
                            {

            
            #line default
            #line hidden
WriteLiteral("                                <tr>\r\n                                    <td>");


            
            #line 39 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
                                   Write(Html.QueueLabel(queue.QueueName));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                                    <td>");


            
            #line 40 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
                                   Write(queue.MessageCount);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                                </tr>\r\n");


            
            #line 42 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
                            }

            
            #line default
            #line hidden
WriteLiteral("                        </tbody>\r\n                    </table>\r\n                <" +
"/div>\r\n            </div>\r\n");


            
            #line 47 "..\..\Dashboard\Pages\FailedQueuePage.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>");


        }
    }
}
#pragma warning restore 1591
