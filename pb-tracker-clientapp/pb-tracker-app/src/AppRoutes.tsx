import LoginPage from './app/login/page';

interface RouteConfiguration {
    path?: string;
    index?: boolean;
    element: React.ReactElement;
}

const AppRoutes: RouteConfiguration[] = [
    {
        path: '/login',
        element: <LoginPage />,
    },
];

export default AppRoutes;
