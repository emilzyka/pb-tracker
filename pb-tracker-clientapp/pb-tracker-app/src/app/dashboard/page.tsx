import { PbTable } from '@/components/PbTable';

export default function Dashboard() {
    return (
        <div className='h-screen w-screen flex items-center justify-center bg-muted px-4'>
            <div className='w-full max-w-md sm:max-w-sm md:max-w-md lg:max-w-md'>
                <PbTable />
            </div>
        </div>
    );
}
