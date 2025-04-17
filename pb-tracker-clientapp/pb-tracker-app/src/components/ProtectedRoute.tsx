import { isAuthenticated } from '@/utils/authUtils';
import { Navigate } from 'react-router';

interface ProtectedRouteProps {
    element: React.ReactElement;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ element }) => {
    return isAuthenticated() ? element : <Navigate to='/login' replace />;
};

export default ProtectedRoute;
