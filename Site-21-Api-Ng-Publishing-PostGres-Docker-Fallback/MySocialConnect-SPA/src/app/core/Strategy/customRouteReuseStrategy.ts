import { ActivatedRouteSnapshot, DetachedRouteHandle, RouteReuseStrategy } from "@angular/router";

/*
Before 
 routeReusestrategy. When the user is on detail page for one user and received a new message toast from user two then
 the page will not refresh the content
 Cannot use below any more hance this new class   
 this.router.routeReuseStrategy.shouldReuseRoute = () => false;
*/
export class CustomRouteReuseStrategy implements RouteReuseStrategy{
    shouldDetach(route: ActivatedRouteSnapshot): boolean {
        return false;
    }

    store(route: ActivatedRouteSnapshot, handle: DetachedRouteHandle | null): void {
        
    }

    shouldAttach(route: ActivatedRouteSnapshot): boolean {
        return false;
    }

    retrieve(route: ActivatedRouteSnapshot): DetachedRouteHandle | null {
        return null;
    }

    shouldReuseRoute(future: ActivatedRouteSnapshot, curr: ActivatedRouteSnapshot): boolean {
        return false;
    }

}