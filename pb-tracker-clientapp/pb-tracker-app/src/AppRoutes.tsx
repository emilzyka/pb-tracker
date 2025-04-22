import Dashboard from './app/dashboard/page';
import LoginPage from './app/login/page';
import RegisterPage from './app/register/page';
import ProtectedRoute from './components/ProtectedRoute';

interface RouteConfiguration {
    path?: string;
    index?: boolean;
    element: React.ReactElement;
}

const AppRoutes: RouteConfiguration[] = [
    {
        path: '/',
        element: <ProtectedRoute element={<Dashboard />} />,
    },
    {
        path: '/login',
        element: <LoginPage />,
    },
    {
        path: '/register',
        element: <RegisterPage />,
    },
];

export default AppRoutes;
