import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Member } from '../_models/member';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { PaginatedResult } from '../_models/pagiantion';
import { Userprams } from '../_models/userPrams';
import { AccountService } from './account.service';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
user : User;
userparams : Userprams;

  constructor(private http: HttpClient , private account : AccountService) {
    this.account.currentUser$.pipe(take(1)).subscribe(user =>
      {
        this.user = user;
        this.userparams = new Userprams(user);
      })
   }

   getUserprams()
   {
     return this.userparams;
   }

   setUserprams(params : Userprams)
   {
     this.userparams = params;
   }

   resetUserprams()
  {
    this.userparams = new Userprams(this.user);
    return this.userparams;
  }

  getMembers(userprams : Userprams) {
    var response = this.memberCache.get(Object.values(userprams).join('-'));
    if(response)
    {
      return of(response);
    }


    let params = this.getPaginationheaders(userprams.pageNumber,userprams.pageSize);

console.log(Object.values(userprams).join('   -   '))

    params = params.append('minAge',userprams.minAge.toString());
    params = params.append('maxAge',userprams.maxAge.toString());
    params = params.append('gender',userprams.gender);
    params = params.append('orderBy',userprams.orderBy);

       return this.getPaginatedResult<Member[]>(this.baseUrl +'users',params).pipe
       (map(res=>{
        this.memberCache.set(Object.values(userprams).join('-'),res);
        return res;
       }));
  }

  private getPaginatedResult<T>(url,params) {
    const paganitadResult : PaginatedResult<T> = new PaginatedResult<T>();

    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paganitadResult.result = response.body;
        if (response.headers.get('pagination') != null) {
          paganitadResult.pagination = JSON.parse(response.headers.get('pagination'));
        }
        return paganitadResult;
      })
    );
  }

  private getPaginationheaders(pageNumber : number , pageSize : number)
  {
     let params = new HttpParams();
      params = params.append('pageNumber',pageNumber.toString());
      params = params.append('pageSize',pageSize.toString());
    return params;
  }

  getMember(username: string) {
    const member = [...this.memberCache.values()].reduce((prev , current)=>
      prev.concat(current.result),[]).find(x=>x.username === username);
   console.log(member);

    if (member !== undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  addLike(username:string)
  {
    return this.http.post(this.baseUrl+'Likes/'+username,{});
  }
  getLikes(predicate:string , pageNumber: number ,pageSize : number)
  {
    let params = this.getPaginationheaders(pageNumber,pageSize)
    params = params.append('predicate',predicate);
    return this.getPaginatedResult<Partial<Member[]>>(this.baseUrl+'Likes',params);
  }

}
