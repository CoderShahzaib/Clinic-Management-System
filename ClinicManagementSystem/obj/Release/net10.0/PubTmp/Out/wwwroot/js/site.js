document.addEventListener('DOMContentLoaded', function () {

    const appointmentsSection = document.getElementById('appointmentsSection');
    const doctorsSection = document.getElementById('doctorsSection');

    const appointmentsTab = document.getElementById('appointmentsTab');
    const doctorsTab = document.getElementById('doctorsTab');

    const todaySection = document.getElementById('todaySection');
    const allSection = document.getElementById('allSection');

    const todayTab = document.getElementById('todayTab');
    const allTab = document.getElementById('allTab');

    const modal = document.getElementById('appointmentModal');

    window.openModal = function () {
        if (!modal) return;
        modal.classList.remove('hidden');
        modal.classList.add('flex');
    };

    window.closeModal = function () {
        if (!modal) return;
        modal.classList.add('hidden');
        modal.classList.remove('flex');
    };

    window.showAppointments = function () {
        if (!appointmentsSection || !doctorsSection) return;

        appointmentsSection.classList.remove('hidden');
        doctorsSection.classList.add('hidden');

        if (appointmentsTab && doctorsTab) {
            appointmentsTab.classList.add('border-purple-600', 'text-purple-600');
            appointmentsTab.classList.remove('border-transparent', 'text-gray-500');

            doctorsTab.classList.add('border-transparent', 'text-gray-500');
            doctorsTab.classList.remove('border-purple-600', 'text-purple-600');
        }
    };

    window.showDoctors = function () {
        if (!appointmentsSection || !doctorsSection) return;

        appointmentsSection.classList.add('hidden');
        doctorsSection.classList.remove('hidden');

        if (appointmentsTab && doctorsTab) {
            doctorsTab.classList.add('border-purple-600', 'text-purple-600');
            doctorsTab.classList.remove('border-transparent', 'text-gray-500');

            appointmentsTab.classList.add('border-transparent', 'text-gray-500');
            appointmentsTab.classList.remove('border-purple-600', 'text-purple-600');
        }
    };

    window.showTodayAppointments = function () {
        if (!todaySection || !allSection) return;

        todaySection.classList.remove('hidden');
        allSection.classList.add('hidden');

        if (todayTab && allTab) {
            todayTab.classList.add('border-green-600', 'text-green-600');
            todayTab.classList.remove('border-transparent', 'text-gray-500');

            allTab.classList.add('border-transparent', 'text-gray-500');
            allTab.classList.remove('border-green-600', 'text-green-600');
        }
    };

    window.showAllAppointments = function () {
        if (!todaySection || !allSection) return;

        allSection.classList.remove('hidden');
        todaySection.classList.add('hidden');

        if (todayTab && allTab) {
            allTab.classList.add('border-green-600', 'text-green-600');
            allTab.classList.remove('border-transparent', 'text-gray-500');

            todayTab.classList.add('border-transparent', 'text-gray-500');
            todayTab.classList.remove('border-green-600', 'text-green-600');
        }
    };

    if (appointmentsSection && doctorsSection) {
        showAppointments();
    }

    if (todaySection && allSection) {
        showTodayAppointments();
    }

});