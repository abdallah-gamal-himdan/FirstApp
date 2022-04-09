import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm : NgForm;
  @HostListener('window:beforeunload',['$event'])unloadNotifications($event:any)
  {
    if(this.editForm.dirty)
    {
      $event.returnValue =true;
    }
  }
 member : Member;
 user : User;
  constructor(private accountService : AccountService , private memberService: MembersService
    ,private toast:ToastrService) {
      accountService.currentUser$.pipe(take(1)).subscribe(u =>
      {
        this.user = u;
      })
   }

  ngOnInit(): void {
   this.loadMemeber();
  }
loadMemeber()
{
  this.memberService.getMember(this.user.username).subscribe(m=>
    {
      this.member = m;
    })
}

OnEditMemeber()
{
  if(!this.editForm.valid)return;
  this.memberService.updateMember(this.member).subscribe(e=>{
    this.toast.success("Updated Successfully");
    this.editForm.reset(this.member);
  });  
}
}
